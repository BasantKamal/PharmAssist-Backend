
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PharmAssist.Core.Entities.Email;
using PharmAssist.Core.Entities.Identity;
using PharmAssist.Core.Entities.OTP;
using PharmAssist.Extensions;
using PharmAssist.MiddleWares;
using PharmAssist.Repository.Data;
using PharmAssist.Repository.Identity;
using PharmAssist.Service;
using StackExchange.Redis;

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

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<OtpService>();
            builder.Services.Configure<OtpConfiguration>(builder.Configuration.GetSection("OtpSettings"));
            builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.AddApplicationServices();

			builder.Services.AddIdentityServices(builder.Configuration);
			builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfiguration"));
			builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailConfig>>().Value);
			builder.Services.Configure<OtpConfiguration>(builder.Configuration.GetSection("OtpConfiguration"));
			builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OtpConfiguration>>().Value);

			builder.Services.AddCors(Options =>
			{
				Options.AddPolicy("MyPolicy", options =>
				{
					options.AllowAnyHeader();
					options.AllowAnyMethod();
					options.AllowAnyOrigin();
				});
			});
			#endregion



            var app = builder.Build();

            #region Update Database 


            using var Scope = app.Services.CreateScope();

            var Services = Scope.ServiceProvider;

            var loggerFactory = Services.GetService<ILoggerFactory>();

            try
            {
                var dbContext = Services.GetRequiredService<StoreContext>();
                await dbContext.Database.MigrateAsync();

                var identityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await identityDbContext.Database.MigrateAsync();
                var userManager = Services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager, roleManager);
                await StoreContextSeed.SeedAsync(dbContext);
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
			app.MapGet("/", () => Results.Ok("PharmAssist API is running"));

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
