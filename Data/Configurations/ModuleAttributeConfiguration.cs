using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class ModuleAttributeConfiguration : IEntityTypeConfiguration<ModuleAttribute>
{
    public void Configure(EntityTypeBuilder<ModuleAttribute> builder)
    {
        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ma => ma.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ma => ma.Type)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        

        builder.HasOne(ma => ma.Module)
            .WithMany(m => m.ModuleAttributes)
            .HasForeignKey(ma => ma.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ma => new { ma.ModuleId, ma.Key })
            .IsUnique();
    }
}