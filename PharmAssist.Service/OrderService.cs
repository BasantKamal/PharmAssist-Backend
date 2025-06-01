using PharmAssist.Core;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Order_Aggregation;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Services;
using PharmAssist.Core.Specifications.Order_Spec;

namespace PharmAssist.Service
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public OrderService(IBasketRepository basketRepository,
			IUnitOfWork unitOfWork)
        {
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			//1.Get Basket From Basket Repo

			var basket=await _basketRepository.GetBasketAsync(basketId);

			//2.Get Selected Items at Basket From Product

			var orderItems=new List<OrderItem>();
			if (basket?.Items.Count > 0)
			{
				foreach(var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					var productItemOredered=new ProductItemOrdered(product.Id,product.Name,product.PictureUrl);
					var orderItem = new OrderItem(productItemOredered, item.Quantity,product.Price);
					orderItems.Add(orderItem);
				}
			}

			//3.Calculate SubTotal = proie of product * quantity

			var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

			//4.Get Delivery Method From DeliveryMethod Repo

			var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

			//5.Create Order

			var order=new Order(buyerEmail,shippingAddress,deliveryMethod,orderItems,subTotal);

			//6.Add Order Locally

			await _unitOfWork.Repository<Order>().Add(order);

			//7.Save Order To Database

			var result= await _unitOfWork.CompleteAsync();
			if (result <= 0) return null;
			return order;
		}

		public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
		{
			var spec=new OrderSpec(buyerEmail,orderId);
			var order=await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
			return order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
		{
			var spec= new OrderSpec(buyerEmail);
			var orders =await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
			return orders;
		}
	}
}
