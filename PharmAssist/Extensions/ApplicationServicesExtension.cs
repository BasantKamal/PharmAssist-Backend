using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmAssist.Core;
using PharmAssist.Core.Services;
using PharmAssist.Errors;
using PharmAssist.Helpers;
using PharmAssist.Repository;
using PharmAssist.Service;


namespace PharmAssist.Extensions
{
	public static class ApplicationServicesExtension
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection Services) 
		{
			Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
			Services.AddScoped<IUnitOfWork, UnitOfWork>();
			//Services.AddScoped(typeof(IOrderService),typeof(OrderService));
			//Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
			Services.AddScoped<IEmailService, EmailService>();
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
