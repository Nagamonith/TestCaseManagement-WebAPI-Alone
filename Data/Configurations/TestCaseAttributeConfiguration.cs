using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestCaseAttributeConfiguration : IEntityTypeConfiguration<TestCaseAttribute>
{
    public void Configure(EntityTypeBuilder<TestCaseAttribute> builder)
    {
        builder.HasKey(t => new { t.TestCaseId, t.Key });

        builder.Property(t => t.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Value)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasOne(t => t.TestCase)
            .WithMany(t => t.TestCaseAttributes)
            .HasForeignKey(t => t.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}