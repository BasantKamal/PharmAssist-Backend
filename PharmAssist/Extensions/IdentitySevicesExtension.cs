using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Repository.Identity;
using PharmAssist.Repository.Services;
using PharmAssist.Service;
using System.Text;


namespace PharmAssist.Extensions
{
	public static class IdentitySevicesExtension
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddScoped<ITokenService, TokenService>();
			services.AddIdentity<AppUser,IdentityRole>()
				    .AddEntityFrameworkStores<AppIdentityDbContext>()
					.AddDefaultTokenProviders();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}) 
					.AddJwtBearer(options =>
					{
						options.TokenValidationParameters = new TokenValidationParameters()
						{
							ValidateIssuer = true,
							ValidIssuer = configuration["JWT:ValidIssuer"],
							ValidateAudience = true,
							ValidAudience= configuration["JWT:ValidAudience"],
							ValidateLifetime= true,
							ValidateIssuerSigningKey = true,
							IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))

						};
					});
			return services;
		}
	}
}
