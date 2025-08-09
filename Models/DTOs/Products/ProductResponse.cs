namespace TestCaseManagement.Api.Models.DTOs.Products;

public class ProductResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int VersionCount { get; set; }
    public int ModuleCount { get; set; }
}