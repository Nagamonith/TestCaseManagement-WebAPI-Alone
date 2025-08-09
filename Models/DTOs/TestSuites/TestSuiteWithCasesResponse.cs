using TestCaseManagement.Api.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class TestSuiteWithCasesResponse : TestSuiteResponse
{
    public List<TestCaseResponse> TestCases { get; set; } = new();
}