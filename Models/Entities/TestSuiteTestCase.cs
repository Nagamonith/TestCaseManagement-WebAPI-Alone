namespace TestCaseManagement.Api.Models.Entities;

public class TestSuiteTestCase
{
    public int Id { get; set; }

    // Relationships
    public string TestSuiteId { get; set; } = string.Empty;
    public TestSuite TestSuite { get; set; } = null!;

    public string TestCaseId { get; set; } = string.Empty;
    public TestCase TestCase { get; set; } = null!;

    public string ModuleId { get; set; } = string.Empty;
    public Module Module { get; set; } = null!;

    // Product Version tracking
    public string? ProductVersionId { get; set; }
    public ProductVersion? ProductVersion { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Execution Fields
    public string Result { get; set; } = "Pending"; // Pass/Fail/Pending
    public string? Actual { get; set; }             // Actual result
    public string? Remarks { get; set; }
    public DateTime? UpdatedAt { get; set; }  // Tester's notes

    // Uploads specific to this test suite execution
    public ICollection<Upload> Uploads { get; set; } = new List<Upload>();
}