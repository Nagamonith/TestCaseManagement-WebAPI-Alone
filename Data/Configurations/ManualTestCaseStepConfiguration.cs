using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class ManualTestCaseStepConfiguration : IEntityTypeConfiguration<ManualTestCaseStep>
{
    public void Configure(EntityTypeBuilder<ManualTestCaseStep> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Steps)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(m => m.ExpectedResult)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasOne(m => m.TestCase)
            .WithMany(t => t.ManualTestCaseSteps)
            .HasForeignKey(m => m.TestCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}