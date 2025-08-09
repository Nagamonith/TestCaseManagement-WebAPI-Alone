using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestSuiteTestCaseService : ITestSuiteTestCaseService
{
    private readonly IGenericRepository<TestSuiteTestCase> _repository;
    private readonly IGenericRepository<TestSuite> _testSuiteRepository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IMapper _mapper;

    public TestSuiteTestCaseService(
        IGenericRepository<TestSuiteTestCase> repository,
        IGenericRepository<TestSuite> testSuiteRepository,
        IGenericRepository<TestCase> testCaseRepository,
        IMapper mapper)
    {
        _repository = repository;
        _testSuiteRepository = testSuiteRepository;
        _testCaseRepository = testCaseRepository;
        _mapper = mapper;
    }

    public async Task<TestSuiteWithCasesResponse> GetAllTestCasesAsync(string testSuiteId)
    {
        var testSuite = await _testSuiteRepository.GetByIdAsync(testSuiteId);
        if (testSuite == null) throw new KeyNotFoundException("Test suite not found");

        var testCases = await _repository.FindAsync(
    t => t.TestSuiteId == testSuiteId,
    q => q.Include(t => t.TestCase));

        var response = _mapper.Map<TestSuiteWithCasesResponse>(testSuite);
        response.TestCases = _mapper.Map<List<TestCaseResponse>>(testCases.Select(t => t.TestCase));

        return response;
    }

    public async Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request)
    {
        var testSuite = await _testSuiteRepository.GetByIdAsync(testSuiteId);
        if (testSuite == null) throw new KeyNotFoundException("Test suite not found");

        var existingTestCases = await _repository.FindAsync(t => t.TestSuiteId == testSuiteId);
        var existingTestCaseIds = existingTestCases.Select(t => t.TestCaseId).ToList();

        var newTestCases = request.TestCaseIds
            .Except(existingTestCaseIds)
            .Distinct()
            .ToList();

        foreach (var testCaseId in newTestCases)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
            if (testCase == null) continue;

            var testSuiteTestCase = new TestSuiteTestCase
            {
                TestSuiteId = testSuiteId,
                TestCaseId = testCaseId,
                ModuleId = testCase.ModuleId,
                Version = testCase.Version
            };

            await _repository.AddAsync(testSuiteTestCase);
        }
    }

    public async Task<bool> RemoveTestCaseAsync(string testSuiteId, string testCaseId)
    {
        var testSuiteTestCase = (await _repository.FindAsync(t =>
            t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId)).FirstOrDefault();

        if (testSuiteTestCase == null) return false;

        _repository.Remove(testSuiteTestCase);
        return true;
    }
}