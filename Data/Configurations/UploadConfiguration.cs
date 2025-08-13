using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestCaseManagement.Api.Models.Entities;

namespace TestCaseManagement.Data.Configurations;

public class UploadConfiguration : IEntityTypeConfiguration<Upload>
{
    public void Configure(EntityTypeBuilder<Upload> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.FilePath)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(u => u.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.UploadedAt)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        // Nullable foreign key relationship
        builder.HasOne(u => u.TestCase)
            .WithMany(t => t.Uploads)
            .HasForeignKey(u => u.TestCaseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
