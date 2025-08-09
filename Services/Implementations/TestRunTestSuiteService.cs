using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestRunTestSuiteService : ITestRunTestSuiteService
{
    private readonly IGenericRepository<TestRunTestSuite> _repository;
    private readonly IGenericRepository<TestRun> _testRunRepository;
    private readonly IGenericRepository<TestSuite> _testSuiteRepository;

    public TestRunTestSuiteService(
        IGenericRepository<TestRunTestSuite> repository,
        IGenericRepository<TestRun> testRunRepository,
        IGenericRepository<TestSuite> testSuiteRepository)
    {
        _repository = repository;
        _testRunRepository = testRunRepository;
        _testSuiteRepository = testSuiteRepository;
    }

    public async Task AssignTestSuitesAsync(string testRunId, AssignTestSuitesRequest request)
    {
        var testRun = await _testRunRepository.GetByIdAsync(testRunId);
        if (testRun == null) throw new KeyNotFoundException("Test run not found");

        var existingTestSuites = await _repository.FindAsync(t => t.TestRunId == testRunId);
        var existingTestSuiteIds = existingTestSuites.Select(t => t.TestSuiteId).ToList();

        var newTestSuites = request.TestSuiteIds
            .Except(existingTestSuiteIds)
            .Distinct()
            .ToList();

        foreach (var testSuiteId in newTestSuites)
        {
            var testSuite = await _testSuiteRepository.GetByIdAsync(testSuiteId);
            if (testSuite == null) continue;

            var testRunTestSuite = new TestRunTestSuite
            {
                TestRunId = testRunId,
                TestSuiteId = testSuiteId
            };

            await _repository.AddAsync(testRunTestSuite);
        }
    }

    public async Task<bool> RemoveTestSuiteAsync(string testRunId, string testSuiteId)
    {
        var testRunTestSuite = (await _repository.FindAsync(t =>
            t.TestRunId == testRunId && t.TestSuiteId == testSuiteId)).FirstOrDefault();

        if (testRunTestSuite == null) return false;

        _repository.Remove(testRunTestSuite);
        return true;
    }
}