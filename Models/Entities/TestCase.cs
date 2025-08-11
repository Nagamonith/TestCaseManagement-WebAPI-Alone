namespace TestCaseManagement.Api.Models.Entities;

public class TestCase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ModuleId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string TestCaseId { get; set; } = string.Empty;
    public string UseCase { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public string TestType { get; set; } = "Manual"; // Default to Manual
    public string? TestTool { get; set; }
    public string? Result { get; set; }
    public string? Actual { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Module Module { get; set; } = null!;
    public ICollection<ManualTestCaseStep> ManualTestCaseSteps { get; set; } = new List<ManualTestCaseStep>();
    public ICollection<TestCaseAttribute> TestCaseAttributes { get; set; } = new List<TestCaseAttribute>();
    public ICollection<Upload> Uploads { get; set; } = new List<Upload>();
    public ICollection<TestSuiteTestCase> TestSuiteTestCases { get; set; } = new List<TestSuiteTestCase>();
    public ICollection<TestRunResult> TestRunResults { get; set; } = new List<TestRunResult>();
}