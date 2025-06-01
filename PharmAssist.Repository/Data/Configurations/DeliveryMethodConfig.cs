using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities.Order_Aggregation;


namespace Talabat.Repository.Data.Configurations
{
	public class DeliveryMethodConfig : IEntityTypeConfiguration<DeliveryMethod>
	{
		public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
		{
			builder.Property(DM => DM.Cost)
				   .HasColumnType("decimal(18,2)");
		}
	}
}
