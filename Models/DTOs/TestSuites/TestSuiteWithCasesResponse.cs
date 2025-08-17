using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class TestSuiteWithCasesResponse : TestSuiteResponse
{
    public List<TestCaseWithExecutionDetailsResponse> TestCases { get; set; } = new();
}

public class TestCaseWithExecutionDetailsResponse
{
    public TestCaseResponse TestCase { get; set; } = null!;
    public ExecutionDetailsResponse ExecutionDetails { get; set; } = null!;
}