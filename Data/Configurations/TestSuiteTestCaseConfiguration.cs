using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations
{
    public class TestSuiteTestCaseConfiguration : IEntityTypeConfiguration<TestSuiteTestCase>
    {
        public void Configure(EntityTypeBuilder<TestSuiteTestCase> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.AddedAt)
                .HasDefaultValueSql("GETDATE()");

            // Cascade delete when TestSuite is deleted
            builder.HasOne(t => t.TestSuite)
                .WithMany(ts => ts.TestSuiteTestCases)
                .HasForeignKey(t => t.TestSuiteId)
                .OnDelete(DeleteBehavior.NoAction);

            // Prevent cascading delete from TestCase to avoid multiple cascade paths
            builder.HasOne(t => t.TestCase)
                .WithMany(tc => tc.TestSuiteTestCases)
                .HasForeignKey(t => t.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Module relationship
            builder.HasOne(t => t.Module)
                .WithMany()
                .HasForeignKey(t => t.ModuleId)
                .OnDelete(DeleteBehavior.NoAction);

            // New: ProductVersion relationship
            builder.HasOne(t => t.ProductVersion)
                .WithMany() // Adjust if you want navigation from ProductVersion side
                .HasForeignKey(t => t.ProductVersionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique index to avoid duplicate TestSuite-TestCase pairs
            builder.HasIndex(t => new { t.TestSuiteId, t.TestCaseId })
                .IsUnique();
        }
    }
}
