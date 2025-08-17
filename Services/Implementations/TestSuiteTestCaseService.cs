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
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class TestSuiteTestCaseService : ITestSuiteTestCaseService
    {
        private readonly IGenericRepository<TestSuiteTestCase> _repository;
        private readonly IGenericRepository<TestCase> _testCaseRepository;
        private readonly IGenericRepository<TestSuite> _testSuiteRepository;
        private readonly IGenericRepository<Upload> _uploadRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TestSuiteTestCaseService> _logger;
        private readonly AppDbContext _dbContext;

        public TestSuiteTestCaseService(
            IGenericRepository<TestSuiteTestCase> repository,
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<TestSuite> testSuiteRepository,
            IGenericRepository<Upload> uploadRepository,
            IMapper mapper,
            ILogger<TestSuiteTestCaseService> logger,
            AppDbContext dbContext)
        {
            _repository = repository;
            _testCaseRepository = testCaseRepository;
            _testSuiteRepository = testSuiteRepository;
            _uploadRepository = uploadRepository;
            _mapper = mapper;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<TestSuiteWithCasesResponse> GetAllTestCasesAsync(string testSuiteId)
        {
            try
            {
                var testSuite = await _dbContext.TestSuites
                    .AsNoTracking()
                    .Include(ts => ts.TestSuiteTestCases)
                        .ThenInclude(tstc => tstc.TestCase)
                            .ThenInclude(tc => tc.ProductVersion)
                    .Include(ts => ts.TestSuiteTestCases)
                        .ThenInclude(tstc => tstc.Uploads)
                    .FirstOrDefaultAsync(ts => ts.Id == testSuiteId);

                if (testSuite == null)
                {
                    _logger.LogWarning("Test suite not found: {TestSuiteId}", testSuiteId);
                    throw new KeyNotFoundException("Test suite not found");
                }

                var response = _mapper.Map<TestSuiteWithCasesResponse>(testSuite);
                response.TestCases = testSuite.TestSuiteTestCases
                    .Select(tstc => new TestCaseWithExecutionDetailsResponse
                    {
                        TestCase = _mapper.Map<TestCaseResponse>(tstc.TestCase),
                        ExecutionDetails = _mapper.Map<ExecutionDetailsResponse>(tstc)
                    })
                    .ToList();

                _logger.LogInformation("Retrieved {Count} test cases for suite {TestSuiteId}",
                    response.TestCases.Count, testSuiteId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting test cases for test suite {TestSuiteId}", testSuiteId);
                throw;
            }
        }

        public async Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request)
        {
            await using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                // Validate test suite exists
                var testSuiteExists = await _testSuiteRepository.ExistsAsync(ts => ts.Id == testSuiteId);
                if (!testSuiteExists)
                {
                    throw new KeyNotFoundException("Test suite not found");
                }

                // Filter invalid/duplicate IDs
                var validTestCaseIds = request.TestCaseIds?
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList() ?? new List<string>();

                if (!validTestCaseIds.Any())
                {
                    _logger.LogWarning("No valid test case IDs provided for assignment");
                    return;
                }

                // Get existing assignments in single query
                var existingTestCaseIds = await _repository.Query()
                    .Where(t => t.TestSuiteId == testSuiteId)
                    .Select(t => t.TestCaseId)
                    .ToListAsync();

                // Get test cases to assign (that aren't already assigned)
                var newTestCaseIds = validTestCaseIds.Except(existingTestCaseIds).ToList();
                if (!newTestCaseIds.Any())
                {
                    _logger.LogInformation("All test cases are already assigned to test suite {TestSuiteId}", testSuiteId);
                    return;
                }

                var testCasesToAssign = await _testCaseRepository.Query()
                    .Include(tc => tc.Module)
                    .Where(tc => newTestCaseIds.Contains(tc.Id))
                    .ToListAsync();

                // Create all assignments in one batch
                var newAssignments = testCasesToAssign.Select(testCase => new TestSuiteTestCase
                {
                    TestSuiteId = testSuiteId,
                    TestCaseId = testCase.Id,
                    ModuleId = testCase.ModuleId,
                    ProductVersionId = testCase.ProductVersionId,
                    AddedAt = DateTime.UtcNow,
                    Result = "Pending"
                }).ToList();

                if (newAssignments.Any())
                {
                    await _repository.AddRangeAsync(newAssignments);
                    await _repository.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Successfully assigned {Count} test cases to test suite {TestSuiteId}",
                        newAssignments.Count, testSuiteId);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error assigning test cases to test suite {TestSuiteId}", testSuiteId);
                throw;
            }
        }

        public async Task<TestSuiteTestCaseResponse> GetTestCaseExecutionDetailsAsync(int testSuiteTestCaseId)
        {
            var tstc = await _repository.Query()
                .AsNoTracking()
                .Include(t => t.TestCase)
                .Include(t => t.Uploads)
                .FirstOrDefaultAsync(t => t.Id == testSuiteTestCaseId);

            if (tstc == null)
            {
                _logger.LogWarning("Test suite test case not found: {TestSuiteTestCaseId}", testSuiteTestCaseId);
                throw new KeyNotFoundException("Test suite test case not found");
            }

            return _mapper.Map<TestSuiteTestCaseResponse>(tstc);
        }

        public async Task UpdateExecutionDetailsAsync(int testSuiteTestCaseId, UpdateExecutionDetailsRequest request)
        {
            var tstc = await _repository.Query()
                .FirstOrDefaultAsync(t => t.Id == testSuiteTestCaseId);

            if (tstc == null)
            {
                _logger.LogWarning("Test suite test case not found: {TestSuiteTestCaseId}", testSuiteTestCaseId);
                throw new KeyNotFoundException("Test suite test case not found");
            }

            tstc.Result = request.Result;
            tstc.Actual = request.Actual;
            tstc.Remarks = request.Remarks;
            tstc.UpdatedAt = DateTime.UtcNow;

            _repository.Update(tstc);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Updated execution details for test suite test case {TestSuiteTestCaseId}", testSuiteTestCaseId);
        }

        public async Task AddExecutionUploadAsync(int testSuiteTestCaseId, AddExecutionUploadRequest request)
        {
            var tstcExists = await _repository.ExistsAsync(t => t.Id == testSuiteTestCaseId);
            if (!tstcExists)
            {
                _logger.LogWarning("Test suite test case not found: {TestSuiteTestCaseId}", testSuiteTestCaseId);
                throw new KeyNotFoundException("Test suite test case not found");
            }

            var upload = new Upload
            {
                FileName = request.FileName,
                FilePath = request.FilePath,
                FileType = request.FileType,
                FileSize = request.FileSize,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = request.UploadedBy,
                TestSuiteTestCaseId = testSuiteTestCaseId
            };

            await _uploadRepository.AddAsync(upload);
            await _uploadRepository.SaveChangesAsync();
            _logger.LogInformation("Added upload for test suite test case {TestSuiteTestCaseId}", testSuiteTestCaseId);
        }

        public async Task<bool> RemoveTestCaseAsync(string testSuiteId, string testCaseId)
        {
            await using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                // Find the assignment with uploads in single query
                var assignment = await _repository.Query()
                    .Include(t => t.Uploads)
                    .FirstOrDefaultAsync(t =>
                        t.TestSuiteId == testSuiteId &&
                        t.TestCaseId == testCaseId);

                if (assignment == null)
                {
                    _logger.LogWarning("Test case assignment not found: Suite={TestSuiteId}, TestCase={TestCaseId}",
                        testSuiteId, testCaseId);
                    return false;
                }

                // Delete related uploads if they exist
                if (assignment.Uploads.Any())
                {
                    _uploadRepository.RemoveRange(assignment.Uploads);
                    await _uploadRepository.SaveChangesAsync();
                }

                // Delete the assignment
                _repository.Remove(assignment);
                await _repository.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Removed test case {TestCaseId} from suite {TestSuiteId}", testCaseId, testSuiteId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to remove test case {TestCaseId} from suite {TestSuiteId}",
                    testCaseId, testSuiteId);
                throw;
            }
        }

        public async Task RemoveAllAssignmentsAsync(string testSuiteId)
        {
            await using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                // Get all assignments with uploads in single query
                var assignments = await _dbContext.TestSuiteTestCases
                    .Include(t => t.Uploads)
                    .Where(t => t.TestSuiteId == testSuiteId)
                    .ToListAsync();

                if (!assignments.Any())
                {
                    _logger.LogInformation("No assignments found for suite {TestSuiteId}", testSuiteId);
                    return;
                }

                // Delete all uploads first
                var allUploads = assignments.SelectMany(a => a.Uploads).ToList();
                if (allUploads.Any())
                {
                    _uploadRepository.RemoveRange(allUploads);
                    await _uploadRepository.SaveChangesAsync();
                }

                // Then delete all assignments
                _repository.RemoveRange(assignments);
                await _repository.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Removed {Count} assignments from suite {TestSuiteId}",
                    assignments.Count, testSuiteId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to remove all assignments for suite {TestSuiteId}", testSuiteId);
                throw;
            }
        }

        public async Task RemoveExecutionUploadAsync(string uploadId)
        {
            await using var transaction = await _repository.BeginTransactionAsync();

            try
            {
                var upload = await _uploadRepository.Query()
                    .FirstOrDefaultAsync(u => u.Id == uploadId && u.TestSuiteTestCaseId != null);

                if (upload == null)
                {
                    _logger.LogWarning("Upload not found: {UploadId}", uploadId);
                    throw new KeyNotFoundException("Upload not found or not associated with test suite execution");
                }

                _uploadRepository.Remove(upload);  // Changed from RemoveAsync to Remove
                await _uploadRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully removed execution upload {UploadId}", uploadId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to remove execution upload {UploadId}", uploadId);
                throw;
            }
        }
    }
}