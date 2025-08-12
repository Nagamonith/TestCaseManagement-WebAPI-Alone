using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestSuiteConfiguration : IEntityTypeConfiguration<TestSuite>
{
    public void Configure(EntityTypeBuilder<TestSuite> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ts => ts.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(ts => ts.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(ts => ts.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(ts => ts.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(ts => ts.Product)
            .WithMany(p => p.TestSuites)
            .HasForeignKey(ts => ts.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ts => ts.TestSuiteTestCases)
            .WithOne(tstc => tstc.TestSuite)
            .HasForeignKey(tstc => tstc.TestSuiteId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(ts => ts.TestRunTestSuites)
            .WithOne(trts => trts.TestSuite)
            .HasForeignKey(trts => trts.TestSuiteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}