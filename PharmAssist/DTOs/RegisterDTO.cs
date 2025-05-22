using System.ComponentModel.DataAnnotations;

namespace PharmAssist.DTOs
{
	public class RegisterDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Name { get; set; }


		[Required]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }


	}
}
