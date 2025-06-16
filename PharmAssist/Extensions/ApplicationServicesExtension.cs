using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.Core;
using PharmAssist.Core.Repositories;
using PharmAssist.Core.Services;
using PharmAssist.Errors;
using PharmAssist.Helpers;
using PharmAssist.Repository;
using PharmAssist.Service;
using StackExchange.Redis;


namespace PharmAssist.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services) 
		{
			// Register cache service with fallback
			Services.AddSingleton<IResponseCacheService>(serviceProvider =>
			{
				var redis = serviceProvider.GetService<IConnectionMultiplexer>();
				if (redis != null && redis.IsConnected)
				{
					return new ResponseCacheService(redis);
				}
				else
				{
					var logger = serviceProvider.GetRequiredService<ILogger<ResponseCacheService>>();
					logger.LogWarning("Redis connection not available. Using in-memory fallback cache service.");
					return new FallbackCacheService();
				}
			});

			Services.AddScoped<IUnitOfWork, UnitOfWork>();
			Services.AddScoped(typeof(IOrderService), typeof(OrderService));
			
			// Register database-based basket repository
			Services.AddScoped<IBasketRepository, DbBasketRepository>();
			
			Services.AddScoped<IEmailService, EmailService>();
			Services.AddScoped<IMedicationRecommendationService, MedicationRecommendationService>();
			Services.Configure<IdentityOptions>(options => options.SignIn.RequireConfirmedEmail = true);


			Services.AddAutoMapper(typeof(MappingProfiles));

			#region Error Handling
			Services.Configure<ApiBehaviorOptions>(Options =>
			{
				Options.InvalidModelStateResponseFactory = (actionContext) =>
				{
				
					var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
													   .SelectMany(p => p.Value.Errors)
													   .Select(e => e.ErrorMessage)
													   .ToArray();

					var ValidationErrorResponse = new ApiValidationErrorResponse()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(ValidationErrorResponse);
				};
			});
			#endregion

			return Services;
		}
	}
}
