

namespace PharmAssist.Core.Entities
{
	public class CustomerBasket
	{
		public CustomerBasket(string id)
		{
			Id= id;
		}

		public string Id { get; set; }
        public List<BasketItem> Items { get; set; }
        public int? DeliveryMethodId { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
    }
}
