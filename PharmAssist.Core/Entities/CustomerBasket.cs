

namespace PharmAssist.Core.Entities
{
	public class CustomerBasket
	{
		public CustomerBasket(string id)
		{
			Id= id;
			Items = new List<BasketItem>();
		}

		public string Id { get; set; }
        public List<BasketItem> Items { get; set; }
    }
}
