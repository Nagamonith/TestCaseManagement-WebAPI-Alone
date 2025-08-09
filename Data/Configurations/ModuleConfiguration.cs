using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.ToTable("Modules");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(m => m.Version)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.CreatedAt)
        .IsRequired() // Explicitly mark as required
        .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(m => m.Product)
            .WithMany(p => p.Modules)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.ProductId, m.Name, m.Version })
            .IsUnique();
        // Add this to the Configure method to ensure proper string handling
        builder.Property(m => m.Version)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);  // Add this for version strings

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);
    }
}