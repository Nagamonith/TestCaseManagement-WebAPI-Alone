using System.Reflection;

namespace TestCaseManagement.Api.Models.Entities;

public class ProductVersion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<Module> Modules { get; set; } = new List<Module>();

    // Added navigation property for related test cases
    public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
}
