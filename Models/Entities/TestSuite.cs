namespace TestCaseManagement.Api.Models.Entities;

public class TestSuite
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
    public ICollection<TestSuiteTestCase> TestSuiteTestCases { get; set; } = new List<TestSuiteTestCase>();
    public ICollection<TestRunTestSuite> TestRunTestSuites { get; set; } = new List<TestRunTestSuite>();
}