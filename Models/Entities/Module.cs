namespace TestCaseManagement.Api.Models.Entities;

public class Module
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Product Product { get; set; } = null!;
    public ICollection<ModuleAttribute> ModuleAttributes { get; set; } = new List<ModuleAttribute>();
    public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
}