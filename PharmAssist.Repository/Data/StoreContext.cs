using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using System.Reflection;


namespace PharmAssist.Repository.Data
{
	public class StoreContext:DbContext
	{
		public StoreContext(DbContextOptions<StoreContext> options):base(options)
		{
				
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Product> Products { get; set; }
		//public DbSet<Order> Orders { get; set; }
		//public DbSet<OrderItem> OrderItems { get; set; }
		//public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
	}
}
