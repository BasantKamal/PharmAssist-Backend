namespace PharmAssist.Core.Entities
{
	public class BasketItem : BaseEntity
	{
		// Required for EF Core
		public BasketItem() { }
		
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string PictureUrl { get; set; }
		public string ActiveIngredient { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		
		// Foreign key for EF Core relationship
		public string BasketId { get; set; }
		public CustomerBasket? Basket { get; set; }
	}
}
