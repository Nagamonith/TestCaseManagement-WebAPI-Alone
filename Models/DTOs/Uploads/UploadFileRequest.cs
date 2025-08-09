using Microsoft.AspNetCore.Http;

namespace TestCaseManagement.Api.Models.DTOs.Uploads;

public class UploadFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string? TestCaseId { get; set; }
}