using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.Responses.Products;

public interface IProductVersionService
{
    Task<IEnumerable<ProductVersionResponse>> GetAllVersionsAsync(string productId);
    Task<ProductVersionResponse?> GetVersionByIdAsync(string productId, string id);
    Task<IdResponse> CreateVersionAsync(string productId, ProductVersionRequest request);
    Task<bool> UpdateVersionAsync(string productId, string id, ProductVersionRequest request);
    Task<bool> DeleteVersionAsync(string productId, string id);
}