using PharmAssist.Core.Entities.Order_Aggregation;

namespace PharmAssist.DTOs
{
    public class AdminOrderDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string DeliveryMethodName { get; set; }
        public decimal DeliveryMethodCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
		public string PaymentIntentId { get; set; }

	}

	public class AdminOrderDetailDTO : AdminOrderDTO
    {
        public AddressDTO ShippingAddress { get; set; }
        public IEnumerable<AdminOrderItemDTO> Items { get; set; }
    }

    public class AdminOrderSummaryDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public int ItemsCount { get; set; }
    }

    public class AdminOrderItemDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public class UpdateOrderStatusDTO
    {
        public OrderStatus Status { get; set; }
    }
}