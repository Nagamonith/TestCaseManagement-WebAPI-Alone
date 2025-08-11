// TestCaseAttributeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

public class TestCaseAttributeConfiguration : IEntityTypeConfiguration<TestCaseAttribute>
{
    public void Configure(EntityTypeBuilder<TestCaseAttribute> builder)
    {
        builder.ToTable("TestCaseAttributes");

        // Composite primary key
        builder.HasKey(ta => new { ta.TestCaseId, ta.ModuleAttributeId });

        builder.Property(ta => ta.Value)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Relationship with TestCase
        builder.HasOne(ta => ta.TestCase)
            .WithMany(tc => tc.TestCaseAttributes)
            .HasForeignKey(ta => ta.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with ModuleAttribute (now required)
        builder.HasOne(ta => ta.ModuleAttribute)
            .WithMany(ma => ma.TestCaseAttributes)
            .HasForeignKey(ta => ta.ModuleAttributeId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if referenced
    }
}