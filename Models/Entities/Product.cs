using System.Reflection;

namespace TestCaseManagement.Api.Models.Entities;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<ProductVersion> ProductVersions { get; set; } = new List<ProductVersion>();
    public ICollection<Module> Modules { get; set; } = new List<Module>();
    public ICollection<TestSuite> TestSuites { get; set; } = new List<TestSuite>();
    public ICollection<TestRun> TestRuns { get; set; } = new List<TestRun>();
}