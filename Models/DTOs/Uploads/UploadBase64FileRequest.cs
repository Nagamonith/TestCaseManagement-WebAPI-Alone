namespace TestCaseManagement.Api.Models.DTOs.Uploads
{
    public class UploadBase64FileRequest
    {
        public string Base64File { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? ContentType { get; set; }
        public string? UploadedBy { get; set; }
        public string? TestCaseId { get; set; }
    }
}
