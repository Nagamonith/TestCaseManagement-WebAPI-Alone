using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations
{
    public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
    {
        public void Configure(EntityTypeBuilder<TestCase> builder)
        {
            builder.HasKey(tc => tc.Id);

            builder.Property(tc => tc.TestCaseId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(tc => tc.UseCase)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(tc => tc.Scenario)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(tc => tc.TestType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(tc => tc.TestTool)
                .HasMaxLength(100);

            builder.Property(tc => tc.Result)
                .HasMaxLength(20)
                .HasConversion<string>();

            builder.Property(tc => tc.Actual)
                .HasColumnType("nvarchar(max)");

            builder.Property(tc => tc.Remarks)
                .HasColumnType("nvarchar(max)");

            builder.Property(tc => tc.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(tc => tc.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Module relationship (cascade delete when module is deleted)
            builder.HasOne(tc => tc.Module)
                .WithMany(m => m.TestCases)
                .HasForeignKey(tc => tc.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductVersion relationship - SetNull on delete
            builder.HasOne(tc => tc.ProductVersion)
       .WithMany(pv => pv.TestCases)
       .HasForeignKey(tc => tc.ProductVersionId)
       .IsRequired(false)
       .OnDelete(DeleteBehavior.SetNull);
            // Changed from Restrict to SetNull

            // Navigation properties cascade deletes
            builder.HasMany(tc => tc.ManualTestCaseSteps)
                .WithOne(m => m.TestCase)
                .HasForeignKey(m => m.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.TestCaseAttributes)
                .WithOne(ta => ta.TestCase)
                .HasForeignKey(ta => ta.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.Uploads)
                .WithOne(u => u.TestCase)
                .HasForeignKey(u => u.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.TestRunResults)
                .WithOne(tr => tr.TestCase)
                .HasForeignKey(tr => tr.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.TestSuiteTestCases)
                .WithOne(tstc => tstc.TestCase)
                .HasForeignKey(tstc => tstc.TestCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(tc => new { tc.ModuleId, tc.TestCaseId, tc.ProductVersionId })
                .IsUnique();
        }
    }
}
