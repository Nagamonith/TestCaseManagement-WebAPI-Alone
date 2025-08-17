using TestCaseManagement.Api.Models.DTOs.Uploads;

namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class ExecutionDetailsResponse
{
    public int TestSuiteTestCaseId { get; set; }
    public string Result { get; set; } = "Pending";
    public string? Actual { get; set; }
    public string? Remarks { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<UploadResponse> Uploads { get; set; } = new();
}