namespace TestCaseManagement.Api.Models.Entities;

public class TestRunResult
{
    public string TestRunId { get; set; } = string.Empty;
    public string TestSuiteId { get; set; } = string.Empty;
    public string TestCaseId { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    public string ExecutedBy { get; set; } = string.Empty;

    public TestRun TestRun { get; set; } = null!;
    public TestSuite TestSuite { get; set; } = null!;
    public TestCase TestCase { get; set; } = null!;
}