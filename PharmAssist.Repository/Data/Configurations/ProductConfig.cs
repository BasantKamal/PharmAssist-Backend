using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities;


namespace PharmAssist.Repository.Data.Configurations
{
	internal class ProductConfig : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			
			builder.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(p => p.Description)
				.IsRequired();
			builder.Property(p=>p.PictureUrl)
				.IsRequired();
			builder.Property(p => p.ActiveIngredient)
				    .IsRequired();
			builder.Property(p => p.Conflicts)
				    .HasMaxLength(500);

			builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
		}
	}
}
