using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestSuiteTestCaseService : ITestSuiteTestCaseService
{
    private readonly IGenericRepository<TestSuiteTestCase> _repository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TestSuiteTestCaseService> _logger;

    public TestSuiteTestCaseService(
        IGenericRepository<TestSuiteTestCase> repository,
        IGenericRepository<TestCase> testCaseRepository,
        IMapper mapper,
        ILogger<TestSuiteTestCaseService> logger)
    {
        _repository = repository;
        _testCaseRepository = testCaseRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TestSuiteWithCasesResponse> GetAllTestCasesAsync(string testSuiteId)
    {
        // Get test cases directly from the repository since we don't have testSuiteRepository
        var testCases = await _repository.FindAsync(
            t => t.TestSuiteId == testSuiteId,
            q => q.Include(t => t.TestCase)
                  .Include(t => t.TestSuite));

        if (!testCases.Any())
        {
            _logger.LogWarning("No test cases found for suite: {TestSuiteId}", testSuiteId);
            throw new KeyNotFoundException("No test cases found for this test suite");
        }

        // Get test suite from the first test case (assuming all belong to same suite)
        var testSuite = testCases.First().TestSuite;
        if (testSuite == null)
        {
            _logger.LogError("Test suite not found in relationships for: {TestSuiteId}", testSuiteId);
            throw new KeyNotFoundException("Test suite not found");
        }

        var response = _mapper.Map<TestSuiteWithCasesResponse>(testSuite);
        response.TestCases = _mapper.Map<List<TestCaseResponse>>(testCases.Select(t => t.TestCase));

        _logger.LogInformation("Retrieved {Count} test cases for suite {TestSuiteId}", response.TestCases.Count, testSuiteId);
        return response;
    }

    public async Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request)
    {
        _logger.LogInformation("Starting test case assignment for suite {TestSuiteId} with {Count} test cases",
            testSuiteId, request.TestCaseIds.Count);

        // Validate all test case IDs and filter out null/empty ones
        var validTestCaseIds = request.TestCaseIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList() ?? new List<string>();

        if (!validTestCaseIds.Any())
        {
            _logger.LogWarning("No valid test case IDs provided for assignment");
            return;
        }

        _logger.LogInformation("Processing {Count} valid test case IDs", validTestCaseIds.Count);

        // Get existing assignments to avoid duplicates
        var existingTestCases = await _repository.FindAsync(t => t.TestSuiteId == testSuiteId);
        var existingTestCaseIds = existingTestCases.Select(t => t.TestCaseId).ToHashSet();

        var newTestCaseIds = validTestCaseIds.Except(existingTestCaseIds).ToList();

        if (!newTestCaseIds.Any())
        {
            _logger.LogInformation("All test cases are already assigned to suite {TestSuiteId}", testSuiteId);
            return;
        }

        _logger.LogInformation("Adding {Count} new test case assignments", newTestCaseIds.Count);

        // Process each new test case ID
        var assignmentsCreated = 0;
        foreach (var testCaseId in newTestCaseIds)
        {
            try
            {
                // Verify test case exists
                var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
                if (testCase == null)
                {
                    _logger.LogWarning("Test case not found, skipping: {TestCaseId}", testCaseId);
                    continue;
                }

                // Create the assignment
                var testSuiteTestCase = new TestSuiteTestCase
                {
                    // Let the database generate the ID (assuming it's configured as identity)
                    TestSuiteId = testSuiteId,
                    TestCaseId = testCaseId,
                    ModuleId = testCase.ModuleId,
                    Version = testCase.Version.ToString(),
                    AddedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(testSuiteTestCase);
                assignmentsCreated++;

                _logger.LogDebug("Created assignment: Suite={TestSuiteId}, TestCase={TestCaseId}",
                    testSuiteId, testCaseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign test case {TestCaseId} to suite {TestSuiteId}",
                    testCaseId, testSuiteId);
            }
        }

        if (assignmentsCreated > 0)
        {
            try
            {
                await _repository.SaveChangesAsync();
                _logger.LogInformation("Successfully assigned {Count} test cases to suite {TestSuiteId}",
                    assignmentsCreated, testSuiteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save test case assignments for suite {TestSuiteId}", testSuiteId);
                throw new InvalidOperationException("Failed to save test case assignments", ex);
            }
        }
    }

    public async Task<bool> RemoveTestCaseAsync(string testSuiteId, string testCaseId)
    {
        _logger.LogInformation("Removing test case {TestCaseId} from suite {TestSuiteId}", testCaseId, testSuiteId);

        var testSuiteTestCase = (await _repository.FindAsync(t =>
            t.TestSuiteId == testSuiteId && t.TestCaseId == testCaseId)).FirstOrDefault();

        if (testSuiteTestCase == null)
        {
            _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                testSuiteId, testCaseId);
            return false;
        }

        try
        {
            _repository.Remove(testSuiteTestCase);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Successfully removed test case {TestCaseId} from suite {TestSuiteId}",
                testCaseId, testSuiteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove test case {TestCaseId} from suite {TestSuiteId}",
                testCaseId, testSuiteId);
            throw new InvalidOperationException("Failed to remove test case assignment", ex);
        }
    }
}