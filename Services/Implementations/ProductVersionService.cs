using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Api.Models.Responses.Products;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class ProductVersionService : IProductVersionService
    {
        private readonly IGenericRepository<ProductVersion> _versionRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public ProductVersionService(
            IGenericRepository<ProductVersion> versionRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper)
        {
            _versionRepository = versionRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVersionResponse>> GetAllVersionsAsync(string productId)
        {
            var versions = await _versionRepository
                .FindAsync(v => v.ProductId == productId);

            return _mapper.Map<IEnumerable<ProductVersionResponse>>(versions);
        }

        public async Task<ProductVersionResponse?> GetVersionByIdAsync(string productId, string id)
        {
            var version = (await _versionRepository
                .FindAsync(v => v.ProductId == productId && v.Id == id))
                .FirstOrDefault();

            return _mapper.Map<ProductVersionResponse>(version);
        }

        public async Task<IdResponse> CreateVersionAsync(string productId, ProductVersionRequest request)
        {
            var productExists = await _productRepository.GetByIdAsync(productId) != null;
            if (!productExists)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var version = _mapper.Map<ProductVersion>(request);
            version.ProductId = productId;
            version.CreatedAt = DateTime.UtcNow;

            await _versionRepository.AddAsync(version);
            await _versionRepository.SaveChangesAsync();

            return new IdResponse { Id = version.Id };
        }

        public async Task<bool> UpdateVersionAsync(string productId, string id, ProductVersionRequest request)
        {
            var version = (await _versionRepository
                .FindAsync(v => v.ProductId == productId && v.Id == id))
                .FirstOrDefault();

            if (version == null)
            {
                return false;
            }

            _mapper.Map(request, version);

            _versionRepository.Update(version);
            await _versionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVersionAsync(string productId, string id)
        {
            var version = (await _versionRepository
                .FindAsync(v => v.ProductId == productId && v.Id == id))
                .FirstOrDefault();

            if (version == null)
            {
                return false;
            }

            _versionRepository.Remove(version);
            await _versionRepository.SaveChangesAsync();

            return true;
        }
    }
}