
using PharmAssist.Core.Entities.Order_Aggregation;

namespace PharmAssist.Core.Specifications.Order_Spec
{
	public class OrderSpec:BaseSpecifications<Order>
	{
        public OrderSpec(string email):base(o=>o.BuyerEmail==email)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
            AddOrderByDesc(o => o.OrderDate);
        }
        public OrderSpec(string email,int orderId) : base(o => o.BuyerEmail == email && o.Id == orderId)
        {
			Includes.Add(o => o.DeliveryMethod);
			Includes.Add(o => o.Items);
		}
    }
}
