using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;

namespace TestCaseManagement.Services.Interfaces;

public interface IProductVersionService
{
    Task<IEnumerable<ProductVersionRequest>> GetAllVersionsAsync(string productId);
    Task<ProductVersionRequest?> GetVersionByIdAsync(string productId, string id);
    Task<IdResponse> CreateVersionAsync(string productId, ProductVersionRequest request);
    Task<bool> UpdateVersionAsync(string productId, string id, ProductVersionRequest request);
    Task<bool> DeleteVersionAsync(string productId, string id);
}