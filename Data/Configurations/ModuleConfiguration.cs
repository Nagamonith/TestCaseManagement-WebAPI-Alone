using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Modules");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(m => m.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.IsActive)
                .HasDefaultValue(true);

            // Product relationship - ClientCascade to avoid multiple cascade paths
            builder.HasOne(m => m.Product)
                .WithMany(p => p.Modules)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.ClientCascade); // Changed here

            // ProductVersion relationship - SetNull when ProductVersion deleted
            builder.HasOne(m => m.ProductVersion)
                .WithMany(pv => pv.Modules)
                .HasForeignKey(m => m.ProductVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Cascade delete ModuleAttributes
            builder.HasMany(m => m.ModuleAttributes)
                .WithOne(ma => ma.Module)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete TestCases
            builder.HasMany(m => m.TestCases)
                .WithOne(tc => tc.Module)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => new { m.ProductId, m.Name, m.ProductVersionId })
                .IsUnique();
        }
    }
}
