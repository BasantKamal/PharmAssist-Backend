using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Order_Aggregation;
using PharmAssist.Core.Entities.Payment;

namespace PharmAssist.Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
        Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
} 