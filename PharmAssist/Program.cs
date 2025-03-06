
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Extensions;
using PharmAssist.MiddleWares;
using PharmAssist.Repository.Identity;

namespace PharmAssist
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);


			#region Configure services Add services to container

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//builder.Services.AddDbContext<StoreContext>(options =>
			//{
			//	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			//});

			builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			//builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
			//{
			//	var connection = builder.Configuration.GetConnectionString("RedisConnection");
			//	return ConnectionMultiplexer.Connect(connection);
			//});

			builder.Services.AddApplicationServices();

			builder.Services.AddIdentityServices(builder.Configuration);
			//builder.Services.AddCors(Options =>
			//{
			//	Options.AddPolicy("MyPolicy", options =>
			//	{
			//		options.AllowAnyHeader();
			//		options.AllowAnyMethod();
			//		options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
			//	});
			//});
			#endregion


			var app = builder.Build();

			#region Update Database 
			//instead of package manager

			//StoreContext dbContext = new StoreContext(); //invalid 3shan msh hynfa3 a3ml ana el options
			//await dbContext.Database.MigrateAsync();

			using var Scope = app.Services.CreateScope();
			//group of services lifetime scoped(dbcontext service)
			var Services = Scope.ServiceProvider;
			//services itself

			var loggerFactory = Services.GetService<ILoggerFactory>();

			try
			{
				//var dbContext = Services.GetRequiredService<StoreContext>(); //ask CLR to create object from dbcontext explicitly
				//await dbContext.Database.MigrateAsync();  //update databse

				var identityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
				await identityDbContext.Database.MigrateAsync();
				var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(UserManager);
				//await StoreContextSeed.SeedAsync(dbContext);


			}
			catch (Exception ex)
			{
				//view in console
				var logger = loggerFactory.CreateLogger<Program>();
				logger.LogError(ex, "An error occured during applying migration");
			}
			#endregion


			#region Configure - Configure the HTTP request pipeline.

			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleWare>();
				app.UseSwaggerMiddleWares();
			}

			app.UseStatusCodePagesWithRedirects("/errors/{0}");
			app.UseHttpsRedirection();
			app.UseStaticFiles(); //for images
			app.UseCors("MyPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			#endregion


			app.Run();
		}
	}
}
