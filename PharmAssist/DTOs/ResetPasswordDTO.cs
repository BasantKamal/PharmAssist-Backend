using System.ComponentModel.DataAnnotations;

namespace PharmAssist.DTOs
{
	public class ResetPasswordDTO
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		public string ResetToken { get; set; }
	}
}
