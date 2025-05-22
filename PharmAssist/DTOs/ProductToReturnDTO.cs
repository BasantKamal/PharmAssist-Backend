
namespace PharmAssist.APIs.DTOs
{
	public class ProductToReturnDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string PictureUrl { get; set; }
		public decimal Price { get; set; }
		public string ActiveIngredient { get; set; }

	}
}
