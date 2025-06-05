using PharmAssist.Core.Entities.Order_Aggregation;

namespace PharmAssist.Core.Specifications.Order_Spec
{
    public class OrderByPaymentIntentIdSpecification : BaseSpecifications<Order>
    {
        public OrderByPaymentIntentIdSpecification(string paymentIntentId) 
            : base(o => o.PaymentIntentId == paymentIntentId)
        {
            Includes.Add(o => o.Items);
            Includes.Add(o => o.DeliveryMethod);
        }
    }
} 