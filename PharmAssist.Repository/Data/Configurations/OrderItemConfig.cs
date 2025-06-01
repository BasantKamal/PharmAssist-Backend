using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities.Order_Aggregation;


namespace PharmAssist.Repository.Data.Configurations
{
	public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.Property(OI => OI.Price)
				   .HasColumnType("decimal(18,2)");

			builder.OwnsOne(OI => OI.Product, p => p.WithOwner());
		}
	}
}
