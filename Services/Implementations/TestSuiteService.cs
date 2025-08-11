using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestSuiteService : ITestSuiteService
{
    private readonly IGenericRepository<TestSuite> _testSuiteRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TestSuiteService> _logger;

    public TestSuiteService(
        IGenericRepository<TestSuite> testSuiteRepository,
        IGenericRepository<Product> productRepository,
        IMapper mapper,
        ILogger<TestSuiteService> logger)
    {
        _testSuiteRepository = testSuiteRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TestSuiteResponse>> GetAllTestSuitesAsync(string productId)
    {
        var testSuites = await _testSuiteRepository.FindAsync(ts => ts.ProductId == productId);
        return _mapper.Map<IEnumerable<TestSuiteResponse>>(testSuites);
    }

    public async Task<TestSuiteResponse?> GetTestSuiteByIdAsync(string productId, string id)
    {
        var testSuite = await _testSuiteRepository.FindAsync(
            filter: ts => ts.ProductId == productId && ts.Id == id,
            include: source => source.Include(ts => ts.TestSuiteTestCases)
                                    .Include(ts => ts.TestRunTestSuites)
        );
        return _mapper.Map<TestSuiteResponse>(testSuite.FirstOrDefault());
    }

    public async Task<IdResponse> CreateTestSuiteAsync(string productId, CreateTestSuiteRequest request)
    {
        var productExists = await _productRepository.ExistsAsync(p => p.Id == productId);
        if (!productExists) throw new KeyNotFoundException("Product not found");

        var testSuite = _mapper.Map<TestSuite>(request);
        testSuite.ProductId = productId;

        await _testSuiteRepository.AddAsync(testSuite);
        await _testSuiteRepository.SaveChangesAsync();

        return new IdResponse { Id = testSuite.Id };
    }

    public async Task<bool> UpdateTestSuiteAsync(string productId, string id, CreateTestSuiteRequest request)
    {
        var testSuite = (await _testSuiteRepository.FindAsync(
            filter: ts => ts.ProductId == productId && ts.Id == id,
            include: source => source.Include(ts => ts.TestSuiteTestCases)
                                    .Include(ts => ts.TestRunTestSuites)
        )).FirstOrDefault();

        if (testSuite == null) return false;

        _mapper.Map(request, testSuite);
        testSuite.UpdatedAt = DateTime.UtcNow;

        _testSuiteRepository.Update(testSuite);
        await _testSuiteRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTestSuiteAsync(string productId, string id, bool forceDelete = false)
    {
        await using var transaction = await _testSuiteRepository.BeginTransactionAsync();

        try
        {
            var testSuite = await _testSuiteRepository.Query()
                .Include(ts => ts.TestSuiteTestCases)
                .Include(ts => ts.TestRunTestSuites)
                .FirstOrDefaultAsync(ts => ts.ProductId == productId && ts.Id == id);

            if (testSuite == null)
            {
                _logger.LogWarning("Test suite not found: ProductId={ProductId}, TestSuiteId={TestSuiteId}",
                    productId, id);
                return false;
            }

            if (!forceDelete && (testSuite.TestSuiteTestCases.Any() || testSuite.TestRunTestSuites.Any()))
            {
                _logger.LogWarning("Attempt to delete referenced test suite without force: {TestSuiteId}", id);
                throw new BadHttpRequestException(
                    "Cannot delete test suite as it is referenced by other entities. Use forceDelete=true to override.");
            }

            // Clear relationships if force deleting
            if (forceDelete)
            {
                var context = _testSuiteRepository.GetDbContext();

                // Remove TestSuiteTestCase relationships
                var testSuiteTestCases = await context.Set<TestSuiteTestCase>()
                    .Where(t => t.TestSuiteId == id)
                    .ToListAsync();
                context.RemoveRange(testSuiteTestCases);

                // Remove TestRunTestSuite relationships
                var testRunTestSuites = await context.Set<TestRunTestSuite>()
                    .Where(t => t.TestSuiteId == id)
                    .ToListAsync();
                context.RemoveRange(testRunTestSuites);
            }

            _testSuiteRepository.Remove(testSuite);
            await _testSuiteRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Successfully deleted test suite {TestSuiteId}", id);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting test suite {TestSuiteId}", id);
            throw;
        }
    }
}