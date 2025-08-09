namespace TestCaseManagement.Api.Models.Entities;

public class TestCaseAttribute
{
    public string TestCaseId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public TestCase TestCase { get; set; } = null!;
}