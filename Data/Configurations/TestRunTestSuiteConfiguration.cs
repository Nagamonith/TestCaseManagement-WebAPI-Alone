using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestRunTestSuiteConfiguration : IEntityTypeConfiguration<TestRunTestSuite>
{
    public void Configure(EntityTypeBuilder<TestRunTestSuite> builder)
    {
        builder.HasKey(tr => new { tr.TestRunId, tr.TestSuiteId });

        builder.HasOne(tr => tr.TestRun)
            .WithMany(t => t.TestRunTestSuites)
            .HasForeignKey(tr => tr.TestRunId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tr => tr.TestSuite)
            .WithMany(t => t.TestRunTestSuites)
            .HasForeignKey(tr => tr.TestSuiteId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}