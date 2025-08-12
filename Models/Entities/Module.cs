namespace TestCaseManagement.Api.Models.Entities
{
    public class Module
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Required relationship with Product
        public string ProductId { get; set; } = string.Empty;
        public Product Product { get; set; } = null!;

        // Optional relationship with ProductVersion
        public string? ProductVersionId { get; set; }
        public ProductVersion? ProductVersion { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<ModuleAttribute> ModuleAttributes { get; set; } = new List<ModuleAttribute>();
        public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    }
}
