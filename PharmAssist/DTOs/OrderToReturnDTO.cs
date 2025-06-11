using PharmAssist.Core.Entities.Order_Aggregation;

namespace PharmAssist.APIs.DTOs
{
	public class OrderToReturnDTO
	{
		public int Id { get; set; }
		public string BuyerEmail { get; set; }
		public DateTimeOffset OrderDate { get; set; }
		public string Status { get; set; } 
		public Address ShippingAddress { get; set; } 
		public string DeliveryMethod { get; set; }  //Name
		public string DeliveryMethodCost { get; set; }  //Cost
		public ICollection<OrderItemDTO> Items { get; set; } = new HashSet<OrderItemDTO>();
		public decimal SubTotal { get; set; } 
		public decimal Total { get; set; }
	}
}
