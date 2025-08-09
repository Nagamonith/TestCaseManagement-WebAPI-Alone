using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestRunResultConfiguration : IEntityTypeConfiguration<TestRunResult>
{
    public void Configure(EntityTypeBuilder<TestRunResult> builder)
    {
        builder.HasKey(tr => new { tr.TestRunId, tr.TestSuiteId, tr.TestCaseId });

        builder.Property(tr => tr.ExecutedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(tr => tr.ExecutedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(tr => tr.TestRun)
            .WithMany(t => t.TestRunResults)
            .HasForeignKey(tr => tr.TestRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tr => tr.TestSuite)
            .WithMany()
            .HasForeignKey(tr => tr.TestSuiteId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(tr => tr.TestCase)
            .WithMany(t => t.TestRunResults)
            .HasForeignKey(tr => tr.TestCaseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}