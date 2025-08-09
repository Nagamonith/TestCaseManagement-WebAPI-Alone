namespace TestCaseManagement.Api.Models.DTOs.TestRuns;

public class AssignTestSuitesRequest
{
    public List<string> TestSuiteIds { get; set; } = new();
}