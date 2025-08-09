using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

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

        builder.HasOne(tc => tc.Module)
            .WithMany(m => m.TestCases)
            .HasForeignKey(tc => tc.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(tc => new { tc.ModuleId, tc.TestCaseId, tc.Version })
            .IsUnique();

    }
    public ICollection<TestRunResult> TestRunResults { get; set; } = new List<TestRunResult>();
}
