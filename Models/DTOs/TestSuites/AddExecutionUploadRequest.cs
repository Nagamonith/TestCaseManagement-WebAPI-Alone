namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class AddExecutionUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}