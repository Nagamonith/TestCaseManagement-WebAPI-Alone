namespace TestCaseManagement.Api.Models.DTOs.TestSuites;

public class CreateTestSuiteRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}