namespace TestCaseManagement.Api.Models.Entities;

public class Upload
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string UploadedBy { get; set; } = string.Empty;

    // Existing TestCase relationship
    public string? TestCaseId { get; set; }
    public TestCase? TestCase { get; set; }

    // New TestSuiteTestCase relationship
    public int? TestSuiteTestCaseId { get; set; }
    public TestSuiteTestCase? TestSuiteTestCase { get; set; }
}