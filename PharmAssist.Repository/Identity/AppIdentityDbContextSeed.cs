

using Microsoft.AspNetCore.Identity;
using PharmAssist.Core.Entities.Identity;

namespace PharmAssist.Repository.Identity
{
	public static class AppIdentityDbContextSeed
	{
		public static async Task SeedUserAsync(UserManager<AppUser> userManager)
		{
			if (!userManager.Users.Any())
			{
				var User = new AppUser()
				{
					DisplayName = "Basant Kamal",
					Email = "basantkamal@gmail.com",
					UserName = "basantkamal",
					PhoneNumber = "01110986580"
				};

				await userManager.CreateAsync(User, "Pa$$w0rd");
			}


		}
	}
}
