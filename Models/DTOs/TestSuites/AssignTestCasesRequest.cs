namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class AssignTestCasesRequest
{
    public List<string> TestCaseIds { get; set; } = new();
}