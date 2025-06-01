using PharmAssist.Core.Entities.Order_Aggregation;


namespace PharmAssist.Core.Services
{
	public interface IOrderService
	{
		Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);
		Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail);
		Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId);
	}
}
