using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;

namespace TestCaseManagement.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> GetProductByIdAsync(string id);
    Task<IdResponse> CreateProductAsync(CreateProductRequest request);
    Task<bool> UpdateProductAsync(string id, UpdateProductRequest request);
    Task<bool> DeleteProductAsync(string id);
}