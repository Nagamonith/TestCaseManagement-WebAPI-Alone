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

            // Configure execution fields
            builder.Property(t => t.Result)
                .HasDefaultValue("Pending")
                .HasMaxLength(50);

            builder.Property(t => t.Actual)
                .HasMaxLength(1000);

            builder.Property(t => t.Remarks)
                .HasMaxLength(1000);

            // Relationships configuration
            builder.HasOne(t => t.TestSuite)
                .WithMany(ts => ts.TestSuiteTestCases)
                .HasForeignKey(t => t.TestSuiteId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.TestCase)
                .WithMany(tc => tc.TestSuiteTestCases)
                .HasForeignKey(t => t.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Module)
                .WithMany()
                .HasForeignKey(t => t.ModuleId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.ProductVersion)
                .WithMany()
                .HasForeignKey(t => t.ProductVersionId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Uploads relationship
            builder.HasMany(t => t.Uploads)
                .WithOne(u => u.TestSuiteTestCase)
                .HasForeignKey(u => u.TestSuiteTestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index to avoid duplicate TestSuite-TestCase pairs
            builder.HasIndex(t => new { t.TestSuiteId, t.TestCaseId })
                .IsUnique();
            builder.Property(t => t.UpdatedAt)
    .IsRequired(false); // Make it nullable since it won't be set initially
        }
    }
}