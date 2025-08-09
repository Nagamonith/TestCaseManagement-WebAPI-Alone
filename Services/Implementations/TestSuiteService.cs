using AutoMapper;
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

    public TestSuiteService(
        IGenericRepository<TestSuite> testSuiteRepository,
        IGenericRepository<Product> productRepository,
        IMapper mapper)
    {
        _testSuiteRepository = testSuiteRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestSuiteResponse>> GetAllTestSuitesAsync(string productId)
    {
        var testSuites = await _testSuiteRepository.FindAsync(ts => ts.ProductId == productId);
        return _mapper.Map<IEnumerable<TestSuiteResponse>>(testSuites);
    }

    public async Task<TestSuiteResponse?> GetTestSuiteByIdAsync(string productId, string id)
    {
        var testSuite = await _testSuiteRepository.FindAsync(ts => ts.ProductId == productId && ts.Id == id);
        return _mapper.Map<TestSuiteResponse>(testSuite.FirstOrDefault());
    }

    public async Task<IdResponse> CreateTestSuiteAsync(string productId, CreateTestSuiteRequest request)
    {
        var productExists = await _productRepository.GetByIdAsync(productId) != null;
        if (!productExists) throw new KeyNotFoundException("Product not found");

        var testSuite = _mapper.Map<TestSuite>(request);
        testSuite.ProductId = productId;

        await _testSuiteRepository.AddAsync(testSuite);
        return new IdResponse { Id = testSuite.Id };
    }

    public async Task<bool> UpdateTestSuiteAsync(string productId, string id, CreateTestSuiteRequest request)
    {
        var testSuite = (await _testSuiteRepository.FindAsync(ts => ts.ProductId == productId && ts.Id == id)).FirstOrDefault();
        if (testSuite == null) return false;

        _mapper.Map(request, testSuite);
        testSuite.UpdatedAt = DateTime.UtcNow;
        _testSuiteRepository.Update(testSuite);
        return true;
    }

    public async Task<bool> DeleteTestSuiteAsync(string productId, string id)
    {
        var testSuite = (await _testSuiteRepository.FindAsync(ts => ts.ProductId == productId && ts.Id == id)).FirstOrDefault();
        if (testSuite == null) return false;

        _testSuiteRepository.Remove(testSuite);
        return true;
    }
}