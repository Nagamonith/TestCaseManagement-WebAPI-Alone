using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
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
            var context = _repository.GetDbContext();
            var testSuite = await context.Set<TestSuite>()
                .Include(ts => ts.TestSuiteTestCases)
                    .ThenInclude(tstc => tstc.TestCase)
                .FirstOrDefaultAsync(ts => ts.Id == testSuiteId);

            if (testSuite == null)
            {
                _logger.LogWarning("Test suite not found: {TestSuiteId}", testSuiteId);
                throw new KeyNotFoundException("Test suite not found");
            }

            var response = _mapper.Map<TestSuiteWithCasesResponse>(testSuite);
            response.TestCases = _mapper.Map<List<TestCaseResponse>>(
                testSuite.TestSuiteTestCases.Select(t => t.TestCase).Where(tc => tc != null)
            );

            _logger.LogInformation("Retrieved {Count} test cases for suite {TestSuiteId}", response.TestCases.Count, testSuiteId);
            return response;
        }

        public async Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request)
        {
            _logger.LogInformation("Starting test case assignment for suite {TestSuiteId} with {Count} test cases",
                testSuiteId, request.TestCaseIds?.Count ?? 0);

            var validTestCaseIds = request.TestCaseIds?
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList() ?? new List<string>();

            if (!validTestCaseIds.Any())
            {
                _logger.LogWarning("No valid test case IDs provided for assignment");
                return;
            }

            var existingTestCases = await _repository.FindAsync(t => t.TestSuiteId == testSuiteId);
            var existingTestCaseIds = existingTestCases.Select(t => t.TestCaseId).ToHashSet();

            var newTestCaseIds = validTestCaseIds.Except(existingTestCaseIds).ToList();
            if (!newTestCaseIds.Any())
            {
                _logger.LogInformation("All test cases are already assigned to suite {TestSuiteId}", testSuiteId);
                return;
            }

            _logger.LogInformation("Adding {Count} new test case assignments", newTestCaseIds.Count);

            var assignmentsCreated = 0;
            foreach (var testCaseId in newTestCaseIds)
            {
                try
                {
                    var singleTestCase = (await _testCaseRepository.FindAsync(tc => tc.Id == testCaseId))
                        .FirstOrDefault();

                    if (singleTestCase == null)
                    {
                        _logger.LogWarning("Test case not found, skipping: {TestCaseId}", testCaseId);
                        continue;
                    }

                    var testSuiteTestCase = new TestSuiteTestCase
                    {
                        TestSuiteId = testSuiteId,
                        TestCaseId = testCaseId,
                        ModuleId = singleTestCase.ModuleId,
                        ProductVersionId = singleTestCase.ProductVersionId,
                        AddedAt = DateTime.UtcNow
                    };

                    await _repository.AddAsync(testSuiteTestCase);
                    assignmentsCreated++;

                    _logger.LogDebug("Created assignment: Suite={TestSuiteId}, TestCase={TestCaseId}, ProductVersionId={ProductVersionId}",
                        testSuiteId, testCaseId, testSuiteTestCase.ProductVersionId);
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

        public async Task RemoveAllAssignmentsAsync(string testSuiteId)
        {
            _logger.LogInformation("Removing all test case assignments from suite {TestSuiteId}", testSuiteId);

            var context = _repository.GetDbContext();
            var assignments = await context.Set<TestSuiteTestCase>()
                .Where(t => t.TestSuiteId == testSuiteId)
                .ToListAsync();

            if (!assignments.Any())
            {
                _logger.LogInformation("No assignments found for suite {TestSuiteId}", testSuiteId);
                return;
            }

            try
            {
                context.RemoveRange(assignments);
                await context.SaveChangesAsync();
                _logger.LogInformation("Removed {Count} assignments from suite {TestSuiteId}", assignments.Count, testSuiteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove all assignments for suite {TestSuiteId}", testSuiteId);
                throw new InvalidOperationException("Failed to remove all test case assignments", ex);
            }
        }
    }
}
