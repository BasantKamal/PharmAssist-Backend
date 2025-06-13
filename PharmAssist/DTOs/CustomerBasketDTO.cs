using System.ComponentModel.DataAnnotations;

namespace PharmAssist.APIs.DTOs
{
	public class CustomerBasketDTO
	{
		public CustomerBasketDTO(string id)
		{
			Id = id;
		}

		[Required]
		public string Id { get; set; }		
		public List<BasketItemDTO> Items { get; set; }

	}
}
