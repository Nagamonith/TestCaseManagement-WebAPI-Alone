using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestSuiteTestCaseConfiguration : IEntityTypeConfiguration<TestSuiteTestCase>
{
    public void Configure(EntityTypeBuilder<TestSuiteTestCase> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.AddedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(t => t.TestSuite)
            .WithMany(t => t.TestSuiteTestCases)
            .HasForeignKey(t => t.TestSuiteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.TestCase)
            .WithMany(t => t.TestSuiteTestCases)
            .HasForeignKey(t => t.TestCaseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(t => t.Module)
            .WithMany()
            .HasForeignKey(t => t.ModuleId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(t => new { t.TestSuiteId, t.TestCaseId })
            .IsUnique();
    }
}