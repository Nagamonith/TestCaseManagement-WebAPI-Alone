namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class ManualTestCaseStepRequest
{
    public string Steps { get; set; } = string.Empty; // Initialize to avoid null
    public string ExpectedResult { get; set; } = string.Empty; // Initialize to avoid null
}
