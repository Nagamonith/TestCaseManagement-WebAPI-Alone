namespace TestCaseManagement.Api.Models.Entities;

public class TestRunTestSuite
{
    public string TestRunId { get; set; } = string.Empty;
    public string TestSuiteId { get; set; } = string.Empty;

    public TestRun TestRun { get; set; } = null!;
    public TestSuite TestSuite { get; set; } = null!;
}