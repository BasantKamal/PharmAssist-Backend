using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace PharmAssist.Service
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration configuration;

		public TokenService(IConfiguration configuration )
        {
			this.configuration = configuration;
		}
        public async Task<string> CreateTokenAsync(AppUser user,UserManager<AppUser> userManager)
		{
			//Payload
			//1.Private claims

			var AuthClaims = new List<Claim>  //claim: properties of user
			{
				new Claim(ClaimTypes.GivenName,user.DisplayName),
				new Claim(ClaimTypes.Email,user.Email)
			};

			var userRoles = await userManager.GetRolesAsync(user);
			foreach (var role in userRoles)
			{
				AuthClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])); //convert to bytes

			var Token = new JwtSecurityToken(
				issuer: configuration["JWT:ValidIssuer"],
				audience: configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
				claims: AuthClaims,
				signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
				);

			return new JwtSecurityTokenHandler().WriteToken(Token);

		}
	}
}
