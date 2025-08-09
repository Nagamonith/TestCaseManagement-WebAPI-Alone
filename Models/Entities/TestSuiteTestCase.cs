namespace TestCaseManagement.Api.Models.Entities;

public class TestSuiteTestCase
{
    public int Id { get; set; }
    public string TestSuiteId { get; set; } = string.Empty;
    public string TestCaseId { get; set; } = string.Empty;
    public string ModuleId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public TestSuite TestSuite { get; set; } = null!;
    public TestCase TestCase { get; set; } = null!;
    public Module Module { get; set; } = null!;
}