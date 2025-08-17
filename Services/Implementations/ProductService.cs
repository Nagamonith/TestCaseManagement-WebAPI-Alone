using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductVersion> _productVersionRepository;
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly IGenericRepository<TestSuite> _testSuiteRepository;
        private readonly IGenericRepository<TestRun> _testRunRepository;
        private readonly IGenericRepository<TestCase> _testCaseRepository;
        private readonly IGenericRepository<TestRunResult> _testRunResultRepository;
        private readonly IGenericRepository<TestSuiteTestCase> _testSuiteTestCaseRepository;
        private readonly IGenericRepository<TestRunTestSuite> _testRunTestSuiteRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductVersion> productVersionRepository,
            IGenericRepository<Module> moduleRepository,
            IGenericRepository<TestSuite> testSuiteRepository,
            IGenericRepository<TestRun> testRunRepository,
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<TestRunResult> testRunResultRepository,
            IGenericRepository<TestSuiteTestCase> testSuiteTestCaseRepository,
            IGenericRepository<TestRunTestSuite> testRunTestSuiteRepository,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _productVersionRepository = productVersionRepository;
            _moduleRepository = moduleRepository;
            _testSuiteRepository = testSuiteRepository;
            _testRunRepository = testRunRepository;
            _testCaseRepository = testCaseRepository;
            _testRunResultRepository = testRunResultRepository;
            _testSuiteTestCaseRepository = testSuiteTestCaseRepository;
            _testRunTestSuiteRepository = testRunTestSuiteRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

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
            await _productRepository.SaveChangesAsync();
            return new IdResponse { Id = product.Id };
        }

        public async Task<bool> UpdateProductAsync(string id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            _mapper.Map(request, product);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            using var transaction = await _productRepository.BeginTransactionAsync();

            try
            {
                // Get product with all related data that needs to be deleted
                var product = await _productRepository.Query()
                    .Include(p => p.ProductVersions)
                    .Include(p => p.Modules)
                        .ThenInclude(m => m.TestCases)
                            .ThenInclude(tc => tc.TestSuiteTestCases)
                    .Include(p => p.Modules)
                        .ThenInclude(m => m.TestCases)
                            .ThenInclude(tc => tc.TestRunResults)
                    .Include(p => p.TestSuites)
                        .ThenInclude(ts => ts.TestSuiteTestCases)
                    .Include(p => p.TestSuites)
                        .ThenInclude(ts => ts.TestRunTestSuites)
                    .Include(p => p.TestRuns)
                        .ThenInclude(tr => tr.TestRunTestSuites)
                    .Include(p => p.TestRuns)
                        .ThenInclude(tr => tr.TestRunResults)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null) return false;

                // Delete all related entities in proper order

                // 1. Delete TestRunResults first (they reference TestCases and TestRuns)
                if (product.TestRuns?.Any() == true)
                {
                    var testRunResults = product.TestRuns
                        .SelectMany(tr => tr.TestRunResults)
                        .ToList();
                    if (testRunResults.Any())
                    {
                        _testRunResultRepository.RemoveRange(testRunResults);
                    }
                }

                // 2. Delete TestRunTestSuites (they reference TestRuns and TestSuites)
                if (product.TestRuns?.Any() == true)
                {
                    var testRunTestSuites = product.TestRuns
                        .SelectMany(tr => tr.TestRunTestSuites)
                        .ToList();
                    if (testRunTestSuites.Any())
                    {
                        _testRunTestSuiteRepository.RemoveRange(testRunTestSuites);
                    }
                }

                // 3. Delete TestRuns
                if (product.TestRuns?.Any() == true)
                {
                    _testRunRepository.RemoveRange(product.TestRuns);
                }

                // 4. Delete TestSuiteTestCases (they reference TestSuites and TestCases)
                if (product.TestSuites?.Any() == true)
                {
                    var testSuiteTestCases = product.TestSuites
                        .SelectMany(ts => ts.TestSuiteTestCases)
                        .ToList();
                    if (testSuiteTestCases.Any())
                    {
                        _testSuiteTestCaseRepository.RemoveRange(testSuiteTestCases);
                    }
                }

                // 5. Delete TestRunTestSuites from TestSuites
                if (product.TestSuites?.Any() == true)
                {
                    var testRunTestSuites = product.TestSuites
                        .SelectMany(ts => ts.TestRunTestSuites)
                        .ToList();
                    if (testRunTestSuites.Any())
                    {
                        _testRunTestSuiteRepository.RemoveRange(testRunTestSuites);
                    }
                }

                // 6. Delete TestSuites
                if (product.TestSuites?.Any() == true)
                {
                    _testSuiteRepository.RemoveRange(product.TestSuites);
                }

                // 7. Delete TestCase-related entities from Modules
                if (product.Modules?.Any() == true)
                {
                    // Get all test cases from all modules
                    var testCases = product.Modules
                        .SelectMany(m => m.TestCases)
                        .ToList();

                    // Delete TestRunResults for these test cases
                    var testCaseResults = testCases
                        .SelectMany(tc => tc.TestRunResults)
                        .ToList();
                    if (testCaseResults.Any())
                    {
                        _testRunResultRepository.RemoveRange(testCaseResults);
                    }

                    // Delete TestSuiteTestCases for these test cases
                    var testCaseSuiteLinks = testCases
                        .SelectMany(tc => tc.TestSuiteTestCases)
                        .ToList();
                    if (testCaseSuiteLinks.Any())
                    {
                        _testSuiteTestCaseRepository.RemoveRange(testCaseSuiteLinks);
                    }

                    // Delete TestCases
                    _testCaseRepository.RemoveRange(testCases);
                }

                // 8. Delete Modules
                if (product.Modules?.Any() == true)
                {
                    _moduleRepository.RemoveRange(product.Modules);
                }

                // 9. Delete ProductVersions
                if (product.ProductVersions?.Any() == true)
                {
                    _productVersionRepository.RemoveRange(product.ProductVersions);
                }

                // 10. Finally delete the product itself
                _productRepository.Remove(product);

                await _productRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                throw;
            }
        }
    }
}
