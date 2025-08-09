namespace TestCaseManagement.Api.Models.DTOs.TestRuns;

public class CreateTestRunRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}