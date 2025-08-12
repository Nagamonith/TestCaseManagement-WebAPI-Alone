namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class ManualTestCaseStepResponse
{
    public string Steps { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
}