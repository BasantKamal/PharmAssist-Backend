using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Services;
using PharmAssist.Errors;
using Stripe;

namespace PharmAssist.Controllers
{
    public class PaymentsController : APIBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _config;

        public PaymentsController(
            IPaymentService paymentService, 
            ILogger<PaymentsController> logger,
            IConfiguration config)
        {
            _paymentService = paymentService;
            _logger = logger;
            _config = config;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) 
                return BadRequest(new ApiResponse(400, "Problem with your basket"));

            return basket;
        }



        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var endpointSecret = _config["StripeSettings:WebhookSecret"];

                // Validate webhook secret is configured
                if (string.IsNullOrEmpty(endpointSecret))
                {
                    _logger.LogError("Stripe webhook secret is not configured");
                    return BadRequest(new ApiResponse(400, "Webhook not configured"));
                }

                // Validate Stripe signature
                if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
                {
                    _logger.LogError("Missing Stripe-Signature header");
                    return BadRequest(new ApiResponse(400, "Invalid request"));
                }

                Event stripeEvent;
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);
                }
                catch (StripeException ex)
                {
                    _logger.LogError(ex, "Failed to verify webhook signature");
                    return BadRequest(new ApiResponse(400, "Invalid signature"));
                }

                _logger.LogInformation($"Processing Stripe webhook: {stripeEvent.Type}");

                // Process the webhook event
                await ProcessWebhookEvent(stripeEvent);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500);
            }
        }

        private async Task ProcessWebhookEvent(Event stripeEvent)
        {
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var succeededIntent = (Stripe.PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation($"Payment succeeded for intent: {succeededIntent.Id}");
                    
                    var successOrder = await _paymentService.UpdateOrderPaymentSucceeded(succeededIntent.Id);
                    if (successOrder != null)
                    {
                        _logger.LogInformation($"Order {successOrder.Id} status updated to PaymentReceived");
                    }
                    break;
                    
                case "payment_intent.payment_failed":
                    var failedIntent = (Stripe.PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation($"Payment failed for intent: {failedIntent.Id}");
                    
                    var failedOrder = await _paymentService.UpdateOrderPaymentFailed(failedIntent.Id);
                    if (failedOrder != null)
                    {
                        _logger.LogInformation($"Order {failedOrder.Id} status updated to PaymentFailed");
                    }
                    break;
                    
                default:
                    _logger.LogInformation($"Unhandled webhook event type: {stripeEvent.Type}");
                    break;
            }
        }
    }
} 