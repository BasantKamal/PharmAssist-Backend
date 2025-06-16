using Microsoft.EntityFrameworkCore;
using PharmAssist.Core.Entities;
using PharmAssist.Core.Entities.Order_Aggregation;
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
			
			// Configure CustomerBasket and BasketItem relationship
			modelBuilder.Entity<CustomerBasket>()
				.HasKey(b => b.Id);
				
			modelBuilder.Entity<BasketItem>()
				.HasOne(i => i.Basket)
				.WithMany(b => b.Items)
				.HasForeignKey(i => i.BasketId)
				.OnDelete(DeleteBehavior.Cascade);

			// Configure decimal precision for Price
			modelBuilder.Entity<BasketItem>()
				.Property(b => b.Price)
				.HasPrecision(18, 2);
				
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<MedicationRecommendation> MedicationRecommendations { get; set; }

		public DbSet<DeliveryMethod> DeliveryMethod { get; set; }
		public DbSet<CustomerBasket> Baskets { get; set; }
		public DbSet<BasketItem> BasketItems { get; set; }
	}
}
