using AutoMapper;
using TestCaseManagement.Api.Constants;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestRunService : ITestRunService
{
    private readonly IGenericRepository<TestRun> _testRunRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public TestRunService(
        IGenericRepository<TestRun> testRunRepository,
        IGenericRepository<Product> productRepository,
        IMapper mapper)
    {
        _testRunRepository = testRunRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestRunResponse>> GetAllTestRunsAsync(string productId)
    {
        var testRuns = await _testRunRepository.FindAsync(tr => tr.ProductId == productId);
        return _mapper.Map<IEnumerable<TestRunResponse>>(testRuns);
    }

    public async Task<TestRunResponse?> GetTestRunByIdAsync(string productId, string id)
    {
        var testRun = await _testRunRepository.FindAsync(tr => tr.ProductId == productId && tr.Id == id);
        return _mapper.Map<TestRunResponse>(testRun.FirstOrDefault());
    }

    public async Task<IdResponse> CreateTestRunAsync(string productId, CreateTestRunRequest request)
    {
        var productExists = await _productRepository.GetByIdAsync(productId) != null;
        if (!productExists) throw new KeyNotFoundException("Product not found");

        var testRun = _mapper.Map<TestRun>(request);
        testRun.ProductId = productId;
        testRun.CreatedBy = "system"; // Replace with actual user from auth context

        await _testRunRepository.AddAsync(testRun);
        return new IdResponse { Id = testRun.Id };
    }

    public async Task<bool> UpdateTestRunStatusAsync(string productId, string id, string status)
    {
        if (!AppConstants.AllowedTestRunStatuses.Contains(status))
            throw new ArgumentException("Invalid test run status");

        var testRun = (await _testRunRepository.FindAsync(tr => tr.ProductId == productId && tr.Id == id)).FirstOrDefault();
        if (testRun == null) return false;

        testRun.Status = status;
        testRun.UpdatedAt = DateTime.UtcNow;
        _testRunRepository.Update(testRun);
        return true;
    }

    public async Task<bool> DeleteTestRunAsync(string productId, string id)
    {
        var testRun = (await _testRunRepository.FindAsync(tr => tr.ProductId == productId && tr.Id == id)).FirstOrDefault();
        if (testRun == null) return false;

        _testRunRepository.Remove(testRun);
        return true;
    }
}