

namespace PharmAssist.Core.Entities
{
	public class BasketItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string PictureUrl { get; set; }
		public string ActiveIngredient { get; set; }

		public decimal Price { get; set; }
		public int Quantity { get; set; }

	}
}
