using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TestRunService(
            IGenericRepository<TestRun> testRunRepository,
            IGenericRepository<Product> productRepository,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _testRunRepository = testRunRepository;
            _productRepository = productRepository;
            _dbContext = dbContext;
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
                var testRuns = await _testRunRepository.FindAsync(tr => tr.ProductId == productId && tr.Id == id);
                var entity = testRuns.FirstOrDefault();
                if (entity == null) return false;

                // Cascade delete: TestRunTestSuites and TestRunResults are configured for cascade on DB and EF

                _testRunRepository.Remove(entity);
                await _testRunRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
