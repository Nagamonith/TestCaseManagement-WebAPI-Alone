namespace TestCaseManagement.Api.Models.Entities;

public class ManualTestCaseStep
{
    public int Id { get; set; }
    public string TestCaseId { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;

    public TestCase TestCase { get; set; } = null!;
}