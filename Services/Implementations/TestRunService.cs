using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestCaseManagement.Api.Constants;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class TestRunService : ITestRunService
    {
        private readonly IGenericRepository<TestRun> _testRunRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<TestRunTestSuite> _testRunTestSuiteRepository;
        private readonly IGenericRepository<TestRunResult> _testRunResultRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<TestRunService> _logger;

        public TestRunService(
            IGenericRepository<TestRun> testRunRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<TestRunTestSuite> testRunTestSuiteRepository,
            IGenericRepository<TestRunResult> testRunResultRepository,
            AppDbContext dbContext,
            IMapper mapper,
            ILogger<TestRunService> logger)
        {
            _testRunRepository = testRunRepository;
            _productRepository = productRepository;
            _testRunTestSuiteRepository = testRunTestSuiteRepository;
            _testRunResultRepository = testRunResultRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
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
            testRun.CreatedBy = "system"; // TODO: Replace with actual authenticated user

            await _testRunRepository.AddAsync(testRun);
            await _testRunRepository.SaveChangesAsync();

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
            await _testRunRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTestRunAsync(string productId, string id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Get test run with all related data that needs to be deleted
                var testRun = await _dbContext.TestRuns
                    .Include(tr => tr.TestRunTestSuites)
                    .Include(tr => tr.TestRunResults)
                    .FirstOrDefaultAsync(tr => tr.ProductId == productId && tr.Id == id);

                if (testRun == null)
                {
                    _logger.LogWarning("Test run not found: ProductId={ProductId}, TestRunId={TestRunId}", productId, id);
                    return false;
                }

                // 1. Delete all TestRunResults (execution results)
                if (testRun.TestRunResults?.Any() == true)
                {
                    _dbContext.TestRunResults.RemoveRange(testRun.TestRunResults);
                }

                // 2. Delete all TestRunTestSuites (test suite assignments)
                if (testRun.TestRunTestSuites?.Any() == true)
                {
                    _dbContext.TestRunTestSuites.RemoveRange(testRun.TestRunTestSuites);
                }

                // 3. Finally delete the test run itself
                _dbContext.TestRuns.Remove(testRun);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully deleted test run {TestRunId}", id);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting test run {TestRunId}", id);
                throw;
            }
        }
    }
}
