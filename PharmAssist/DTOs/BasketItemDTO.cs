using System.ComponentModel.DataAnnotations;

namespace PharmAssist.APIs.DTOs
{
	public class BasketItemDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string PictureUrl { get; set; }
		public string ActiveIngredient { get; set; }

		[Range(0.1, double.MaxValue,ErrorMessage ="Price Can't Be Zero")] 
		public decimal Price { get; set; }

		[Range(1,int.MaxValue,ErrorMessage ="Quantity Must Be one item at least")]
		public int Quantity { get; set; }
	}
}