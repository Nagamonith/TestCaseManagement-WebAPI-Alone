namespace TestCaseManagement.Api.Models.DTOs.Uploads
{
    public class UploadResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public string? TestCaseId { get; set; }

        // Add this property
        public string FilePath { get; set; } = string.Empty;
    }
}
