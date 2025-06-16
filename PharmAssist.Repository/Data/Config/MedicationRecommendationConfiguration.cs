using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmAssist.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmAssist.Repository.Data.Config
{
    public class MedicationRecommendationConfiguration : IEntityTypeConfiguration<MedicationRecommendation>
    {
        public void Configure(EntityTypeBuilder<MedicationRecommendation> builder)
        {
            builder.Property(m => m.UserId)
                .IsRequired()
                .HasMaxLength(450); // Standard for AspNetCore Identity user IDs

            builder.Property(m => m.SafetyScore)
                .HasPrecision(3, 2); // Allow values like 5.00

            builder.Property(m => m.EffectivenessScore)
                .HasPrecision(3, 2);

            builder.Property(m => m.ValueScore)
                .HasPrecision(3, 2);

            builder.Property(m => m.FinalScore)
                .HasPrecision(3, 2);

            builder.Property(m => m.ConflictDetails)
                .HasMaxLength(500);

            builder.Property(m => m.RecommendationReason)
                .HasMaxLength(1000);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .HasDefaultValue(true);

            // Configure relationship with Product
            builder.HasOne(m => m.Product)
                .WithMany()
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes for better query performance
            builder.HasIndex(m => m.UserId);
            builder.HasIndex(m => new { m.UserId, m.IsActive });
            builder.HasIndex(m => new { m.UserId, m.HasConflict });
            builder.HasIndex(m => m.CreatedAt);
        }
    }
} 