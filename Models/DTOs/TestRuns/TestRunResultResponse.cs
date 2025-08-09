namespace TestCaseManagement.Api.Models.DTOs.TestRuns;

public class TestRunResultResponse
{
    public string TestRunId { get; set; } = string.Empty;
    public string TestSuiteId { get; set; } = string.Empty;
    public string TestCaseId { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public string ExecutedBy { get; set; } = string.Empty;
}