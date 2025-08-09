using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(pv => pv.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(pv => pv.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(pv => new { pv.ProductId, pv.Version })
            .IsUnique();

        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.ProductVersions)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}