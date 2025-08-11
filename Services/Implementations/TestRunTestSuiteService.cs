using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class TestRunTestSuiteService : ITestRunTestSuiteService
    {
        private readonly IGenericRepository<TestRunTestSuite> _repository;
        private readonly IGenericRepository<TestRun> _testRunRepository;
        private readonly IGenericRepository<TestSuite> _testSuiteRepository;
        private readonly IMapper _mapper;

        public TestRunTestSuiteService(
            IGenericRepository<TestRunTestSuite> repository,
            IGenericRepository<TestRun> testRunRepository,
            IGenericRepository<TestSuite> testSuiteRepository,
            IMapper mapper)
        {
            _repository = repository;
            _testRunRepository = testRunRepository;
            _testSuiteRepository = testSuiteRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TestRunTestSuiteResponse>> GetAllTestSuitesAsync(string testRunId)
        {
            var testRunTestSuites = await _repository.FindAsync(
                t => t.TestRunId == testRunId,
                include: query => query.Include(t => t.TestSuite));

            return testRunTestSuites
                .Select(t => _mapper.Map<TestRunTestSuiteResponse>(t.TestSuite))
                .ToList();
        }

        public async Task AssignTestSuitesAsync(string testRunId, AssignTestSuitesRequest request)
        {
            var testRun = await _testRunRepository.GetByIdAsync(testRunId);
            if (testRun == null) throw new KeyNotFoundException("Test run not found");

            var existingTestSuites = await _repository.FindAsync(t => t.TestRunId == testRunId);
            var existingIds = existingTestSuites.Select(t => t.TestSuiteId).ToList();

            var newTestSuites = request.TestSuiteIds
                .Except(existingIds)
                .Distinct()
                .ToList();

            foreach (var id in newTestSuites)
            {
                var testSuite = await _testSuiteRepository.GetByIdAsync(id);
                if (testSuite == null) continue;

                await _repository.AddAsync(new TestRunTestSuite
                {
                    TestRunId = testRunId,
                    TestSuiteId = id
                });
            }
        }

        public async Task<bool> UpdateTestSuiteAsync(string testRunId, string testSuiteId, UpdateTestSuiteRequest request)
        {
            var newTestSuite = await _testSuiteRepository.GetByIdAsync(request.NewTestSuiteId);
            if (newTestSuite == null) return false;

            var existing = (await _repository.FindAsync(t =>
                t.TestRunId == testRunId && t.TestSuiteId == testSuiteId)).FirstOrDefault();

            if (existing == null) return false;

            var alreadyExists = (await _repository.FindAsync(t =>
                t.TestRunId == testRunId && t.TestSuiteId == request.NewTestSuiteId)).Any();

            if (alreadyExists) return false;

            _repository.Remove(existing);

            await _repository.AddAsync(new TestRunTestSuite
            {
                TestRunId = testRunId,
                TestSuiteId = request.NewTestSuiteId
            });

            return true;
        }

        public async Task<bool> RemoveTestSuiteAsync(string testRunId, string testSuiteId)
        {
            var existing = (await _repository.FindAsync(t =>
                t.TestRunId == testRunId && t.TestSuiteId == testSuiteId)).FirstOrDefault();

            if (existing == null) return false;

            _repository.Remove(existing);
            return true;
        }
    }
}
