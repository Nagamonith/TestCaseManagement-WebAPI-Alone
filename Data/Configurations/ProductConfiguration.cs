using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasColumnType("nvarchar(max)");
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
        builder.Property(p => p.IsActive).HasDefaultValue(true);

        builder.HasMany(p => p.ProductVersions)
            .WithOne(pv => pv.Product)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}