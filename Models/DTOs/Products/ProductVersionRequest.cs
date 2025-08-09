namespace TestCaseManagement.Api.Models.DTOs.Products;

public class ProductVersionRequest
{
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}