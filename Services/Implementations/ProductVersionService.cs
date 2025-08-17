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
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly IGenericRepository<TestCase> _testCaseRepository;
        private readonly IGenericRepository<TestSuiteTestCase> _testSuiteTestCaseRepository;
        private readonly IMapper _mapper;

        public ProductVersionService(
            IGenericRepository<ProductVersion> versionRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<Module> moduleRepository,
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<TestSuiteTestCase> testSuiteTestCaseRepository,
            IMapper mapper)
        {
            _versionRepository = versionRepository;
            _productRepository = productRepository;
            _moduleRepository = moduleRepository;
            _testCaseRepository = testCaseRepository;
            _testSuiteTestCaseRepository = testSuiteTestCaseRepository;
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
            using var transaction = await _versionRepository.BeginTransactionAsync();

            try
            {
                // Get version with all related data
                var version = await _versionRepository.Query()
                    .Include(v => v.Modules)
                    .Include(v => v.TestCases)
                    .Include(v => v.TestSuiteTestCases)
                    .FirstOrDefaultAsync(v => v.ProductId == productId && v.Id == id);

                if (version == null)
                {
                    return false;
                }

                // 1. Update Modules to set ProductVersionId to null
                if (version.Modules?.Any() == true)
                {
                    foreach (var module in version.Modules)
                    {
                        module.ProductVersionId = null;
                        _moduleRepository.Update(module);
                    }
                }

                // 2. Update TestCases to set ProductVersionId to null
                if (version.TestCases?.Any() == true)
                {
                    foreach (var testCase in version.TestCases)
                    {
                        testCase.ProductVersionId = null;
                        _testCaseRepository.Update(testCase);
                    }
                }

                // 3. Update TestSuiteTestCases to set ProductVersionId to null
                if (version.TestSuiteTestCases?.Any() == true)
                {
                    foreach (var testSuiteTestCase in version.TestSuiteTestCases)
                    {
                        testSuiteTestCase.ProductVersionId = null;
                        _testSuiteTestCaseRepository.Update(testSuiteTestCase);
                    }
                }

                // 4. Finally delete the version itself
                _versionRepository.Remove(version);

                await _versionRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // Re-throw the exception after rollback
            }
        }
    }
}