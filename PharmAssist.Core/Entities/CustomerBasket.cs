namespace PharmAssist.Core.Entities
{
	public class CustomerBasket
	{
		// Required for EF Core
		protected CustomerBasket() { }
		
		public CustomerBasket(string id)
		{
			Id= id;
			Items = new List<BasketItem>();
		}

		public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new();
    }
}
