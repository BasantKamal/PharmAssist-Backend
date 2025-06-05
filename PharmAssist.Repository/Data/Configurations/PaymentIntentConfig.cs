using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities.Payment;

namespace PharmAssist.Repository.Data.Configurations
{
    public class PaymentIntentConfig : IEntityTypeConfiguration<PaymentIntent>
    {
        public void Configure(EntityTypeBuilder<PaymentIntent> builder)
        {
            builder.Property(p => p.StripePaymentIntentId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("usd");

            builder.Property(p => p.BuyerEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.BasketId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.ClientSecret)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedDate)
                .IsRequired();

            builder.HasIndex(p => p.StripePaymentIntentId)
                .IsUnique();

            builder.HasIndex(p => p.BasketId);
        }
    }
} 