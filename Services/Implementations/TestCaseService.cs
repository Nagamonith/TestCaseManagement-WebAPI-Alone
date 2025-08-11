using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Data;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations
{
    public class TestCaseService : ITestCaseService
    {
        private readonly IGenericRepository<TestCase> _testCaseRepository;
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly IGenericRepository<ModuleAttribute> _moduleAttributeRepository;
        private readonly IGenericRepository<TestCaseAttribute> _testCaseAttributeRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TestCaseService(
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<Module> moduleRepository,
            IGenericRepository<ModuleAttribute> moduleAttributeRepository,
            IGenericRepository<TestCaseAttribute> testCaseAttributeRepository,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _testCaseRepository = testCaseRepository;
            _moduleRepository = moduleRepository;
            _moduleAttributeRepository = moduleAttributeRepository;
            _testCaseAttributeRepository = testCaseAttributeRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TestCaseResponse>> GetAllTestCasesAsync(string moduleId)
        {
            var testCases = await _testCaseRepository.FindAsync(
                tc => tc.ModuleId == moduleId,
                include: query => query
                    .Include(tc => tc.ManualTestCaseSteps)
                    .Include(tc => tc.TestCaseAttributes)
                        .ThenInclude(a => a.ModuleAttribute)
            );

            return _mapper.Map<IEnumerable<TestCaseResponse>>(testCases);
        }

        public async Task<TestCaseDetailResponse?> GetTestCaseByIdAsync(string moduleId, string id)
        {
            var testCase = (await _testCaseRepository.FindAsync(
                tc => tc.ModuleId == moduleId && tc.Id == id,
                include: query => query
                    .Include(tc => tc.ManualTestCaseSteps)
                    .Include(tc => tc.TestCaseAttributes)
                        .ThenInclude(a => a.ModuleAttribute)
            )).FirstOrDefault();

            return _mapper.Map<TestCaseDetailResponse>(testCase);
        }

        public async Task<IdResponse> CreateTestCaseAsync(CreateTestCaseRequest request)
        {
            var moduleExists = await _moduleRepository.GetByIdAsync(request.ModuleId) != null;
            if (!moduleExists)
                throw new KeyNotFoundException("Module not found");

            var testCase = _mapper.Map<TestCase>(request);

            testCase.ManualTestCaseSteps = new List<ManualTestCaseStep>();
            testCase.TestCaseAttributes = new List<TestCaseAttribute>();

            foreach (var stepRequest in request.Steps)
            {
                testCase.ManualTestCaseSteps.Add(new ManualTestCaseStep
                {
                    Steps = stepRequest.Steps,
                    ExpectedResult = stepRequest.ExpectedResult,
                    TestCaseId = testCase.Id
                });
            }

            await _testCaseRepository.AddAsync(testCase);
            return new IdResponse { Id = testCase.Id };
        }

        public async Task<bool> UpdateTestCaseAsync(string moduleId, string id, UpdateTestCaseRequest request)
        {
            var testCase = (await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId && tc.Id == id)).FirstOrDefault();
            if (testCase == null) return false;

            _mapper.Map(request, testCase);
            testCase.UpdatedAt = DateTime.UtcNow;
            _testCaseRepository.Update(testCase);
            return true;
        }

        public async Task<bool> DeleteTestCaseAsync(string moduleId, string id)
        {
            var testCase = (await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId && tc.Id == id)).FirstOrDefault();
            if (testCase == null) return false;

            _testCaseRepository.Remove(testCase);
            return true;
        }

        public async Task<IEnumerable<TestCaseAttributeResponse>> GetTestCaseAttributesAsync(string moduleId, string testCaseId)
        {
            var testCase = (await _testCaseRepository.FindAsync(
                tc => tc.Id == testCaseId && tc.ModuleId == moduleId,
                include: query => query
                    .Include(tc => tc.TestCaseAttributes)
                        .ThenInclude(ta => ta.ModuleAttribute)
            )).FirstOrDefault();

            if (testCase == null)
                return Enumerable.Empty<TestCaseAttributeResponse>();

            return testCase.TestCaseAttributes.Select(ta => new TestCaseAttributeResponse
            {
                Key = ta.ModuleAttribute.Key,
                Value = ta.Value,
                Name = ta.ModuleAttribute.Name,
                Type = ta.ModuleAttribute.Type,
                IsRequired = ta.ModuleAttribute.IsRequired
            });
        }

        public async Task<bool> UpdateTestCaseAttributesAsync(string moduleId, string testCaseId, IEnumerable<TestCaseAttributeRequest> attributes)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
                if (testCase == null || testCase.ModuleId != moduleId)
                    return false;

                var moduleAttributes = await _moduleAttributeRepository
                    .FindAsync(ma => ma.ModuleId == moduleId);

                var existingAttributes = await _testCaseAttributeRepository
                    .FindAsync(ta => ta.TestCaseId == testCaseId);

                foreach (var attrRequest in attributes)
                {
                    var moduleAttr = moduleAttributes.FirstOrDefault(ma => ma.Key == attrRequest.Key);
                    if (moduleAttr == null) continue;

                    var existingAttr = existingAttributes.FirstOrDefault(ta =>
                        ta.ModuleAttributeId == moduleAttr.Id);

                    if (existingAttr != null)
                    {
                        existingAttr.Value = attrRequest.Value;
                        _testCaseAttributeRepository.Update(existingAttr);
                    }
                    else
                    {
                        var newAttr = new TestCaseAttribute
                        {
                            TestCaseId = testCaseId,
                            ModuleAttributeId = moduleAttr.Id,
                            Value = attrRequest.Value
                        };
                        await _testCaseAttributeRepository.AddAsync(newAttr);
                    }
                }

                await _dbContext.SaveChangesAsync();
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