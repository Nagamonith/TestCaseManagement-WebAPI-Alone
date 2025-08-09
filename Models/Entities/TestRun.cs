namespace TestCaseManagement.Api.Models.Entities;

public class TestRun
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Not Started";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;

    public Product Product { get; set; } = null!;
    public ICollection<TestRunTestSuite> TestRunTestSuites { get; set; } = new List<TestRunTestSuite>();

    public ICollection<TestRunResult> TestRunResults { get; set; } = new List<TestRunResult>();
}
