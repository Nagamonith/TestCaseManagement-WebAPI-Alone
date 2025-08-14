// File: Models/Responses/Products/ProductVersionResponse.cs
namespace TestCaseManagement.Api.Models.Responses.Products;

public class ProductVersionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}