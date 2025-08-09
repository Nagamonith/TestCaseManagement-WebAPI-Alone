namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class CreateTestCaseRequest
{
    public string ModuleId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string TestCaseId { get; set; } = string.Empty;
    public string UseCase { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string? TestTool { get; set; }
}