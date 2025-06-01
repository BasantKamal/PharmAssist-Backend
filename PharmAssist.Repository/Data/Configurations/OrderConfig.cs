using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities.Order_Aggregation;


namespace PharmAssist.Repository.Data.Configurations
{
	public class OrderConfig : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(o => o.Status)
				   .HasConversion(OStatus => OStatus.ToString(), OStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus));
			builder.Property(o => o.SubTotal)
				   .HasColumnType("decimal(18,2)");
			builder.OwnsOne(o => o.ShippingAddress, shipAdd => shipAdd.WithOwner());

			builder.HasOne(o => o.DeliveryMethod)
				   .WithMany()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
