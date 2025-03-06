using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities.Identity;
using System.Security.Claims;


namespace PharmAssist.Extensions
{
	public static class UserManagerExtension
	{
		public static async Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser> userManager , ClaimsPrincipal User)
		{
			var email=User.FindFirstValue(ClaimTypes.Email);
			var user= await userManager.Users.Include(u=>u.Address).FirstOrDefaultAsync(u=>u.Email==email); //ngeb el addresses kolha b3d kda bta3t el email ely 3ayzeno
			return user;
		}
	}
}
