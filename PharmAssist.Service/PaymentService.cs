using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Order_Aggregation;
using PharmAssist.Core.Entities.Payment;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Services;
using PharmAssist.Core.Specifications.Order_Spec;
using Stripe;

namespace PharmAssist.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IConfiguration config,
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger)
        {
            _config = config;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Cost;
            }

            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();

            Stripe.PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            { 
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (order == null)
            {
                _logger.LogWarning($"No order found for payment intent: {paymentIntentId}");
                return null;
            }

            order.Status = OrderStatus.PaymentReceived;
            await _unitOfWork.CompleteAsync();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);

            if (order == null)
            {
                _logger.LogWarning($"No order found for payment intent: {paymentIntentId}");
                return null;
            }

            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
} 