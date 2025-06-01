

namespace PharmAssist.Core.Entities.Order_Aggregation
{
	public class Order:BaseEntity
	{
		public Order() { }
		public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
		{
			BuyerEmail = buyerEmail;
			ShippingAddress = shippingAddress;
			DeliveryMethod = deliveryMethod;
			Items = items;
			SubTotal = subTotal;
		}

		public string BuyerEmail { get; set; }
		public DateTimeOffset OrderDate { get; set; }=DateTimeOffset.Now;	 
		public OrderStatus Status { get; set; }=OrderStatus.Pending;
		public Address ShippingAddress { get; set; } //aggregate => msh htthwel
		public DeliveryMethod DeliveryMethod { get; set; } //navigational propety => htthwel l table fel db
		public ICollection<OrderItem> Items { get; set; }=new HashSet<OrderItem>();
		public decimal SubTotal { get; set; } //price of product * quantity
		public decimal GetTotal => SubTotal + DeliveryMethod.Cost;  //not mapped in db
		public string PaymentIntentId { get; set; }=string .Empty;
    }
}
