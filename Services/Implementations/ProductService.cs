using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IGenericRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    // In ProductService.cs
    public async Task<ProductResponse?> GetProductByIdAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        var response = _mapper.Map<ProductResponse>(product);
        response.VersionCount = product.ProductVersions?.Count ?? 0;
        response.ModuleCount = product.Modules?.Count ?? 0;

        return response;
    }

    public async Task<IdResponse> CreateProductAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        await _productRepository.AddAsync(product);
        return new IdResponse { Id = product.Id };
    }

    public async Task<bool> UpdateProductAsync(string id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        _mapper.Map(request, product);
        _productRepository.Update(product);
        return true;
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return false;

        _productRepository.Remove(product);
        return true;
    }
}