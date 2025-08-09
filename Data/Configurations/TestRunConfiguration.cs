using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class TestRunConfiguration : IEntityTypeConfiguration<TestRun>
{
    public void Configure(EntityTypeBuilder<TestRun> builder)
    {
        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tr => tr.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(tr => tr.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(tr => tr.CreatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(tr => tr.UpdatedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(tr => tr.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(tr => tr.Product)
            .WithMany(p => p.TestRuns)
            .HasForeignKey(tr => tr.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}