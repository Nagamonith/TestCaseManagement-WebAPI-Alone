using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class TestSuiteTestCaseResponse
{
    public int Id { get; set; }
    public string TestSuiteId { get; set; } = string.Empty;
    public TestCaseResponse TestCase { get; set; } = null!;
    public ExecutionDetailsResponse ExecutionDetails { get; set; } = null!;
}