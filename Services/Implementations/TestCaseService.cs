using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IGenericRepository<TestSuiteTestCase> _testSuiteTestCaseRepository;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public TestCaseService(
            IGenericRepository<TestCase> testCaseRepository,
            IGenericRepository<Module> moduleRepository,
            IGenericRepository<ModuleAttribute> moduleAttributeRepository,
            IGenericRepository<TestCaseAttribute> testCaseAttributeRepository,
            IGenericRepository<TestSuiteTestCase> testSuiteTestCaseRepository,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _testCaseRepository = testCaseRepository;
            _moduleRepository = moduleRepository;
            _moduleAttributeRepository = moduleAttributeRepository;
            _testCaseAttributeRepository = testCaseAttributeRepository;
            _testSuiteTestCaseRepository = testSuiteTestCaseRepository;
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

            // Map ProductVersionId explicitly if not handled by AutoMapper
            testCase.ProductVersionId = request.ProductVersionId;

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
            await _dbContext.SaveChangesAsync();
            return new IdResponse { Id = testCase.Id };
        }

        public async Task<bool> UpdateTestCaseAsync(string moduleId, string id, UpdateTestCaseRequest request)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var testCase = (await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId && tc.Id == id,
                    include: q => q.Include(tc => tc.ManualTestCaseSteps)
                                   .Include(tc => tc.TestCaseAttributes))).FirstOrDefault();

                if (testCase == null) return false;

                // Map basic fields
                _mapper.Map(request, testCase);

                // Explicitly update ProductVersionId if not mapped by AutoMapper
                testCase.ProductVersionId = request.ProductVersionId;

                testCase.UpdatedAt = DateTime.UtcNow;

                // === Handle Steps update ===
                if (request.Steps != null)
                {
                    var existingSteps = testCase.ManualTestCaseSteps.ToList();

                    foreach (var stepReq in request.Steps)
                    {
                        if (stepReq.Id.HasValue)
                        {
                            var existingStep = existingSteps.FirstOrDefault(s => s.Id == stepReq.Id.Value);
                            if (existingStep != null)
                            {
                                existingStep.Steps = stepReq.Steps;
                                existingStep.ExpectedResult = stepReq.ExpectedResult;
                            }
                        }
                        else
                        {
                            var newStep = new ManualTestCaseStep
                            {
                                Steps = stepReq.Steps,
                                ExpectedResult = stepReq.ExpectedResult,
                                TestCaseId = testCase.Id
                            };
                            testCase.ManualTestCaseSteps.Add(newStep);
                        }
                    }

                    var stepIdsInRequest = request.Steps.Where(s => s.Id.HasValue).Select(s => s.Id.Value).ToHashSet();
                    var stepsToRemove = existingSteps.Where(s => !stepIdsInRequest.Contains(s.Id)).ToList();

                    foreach (var stepToRemove in stepsToRemove)
                    {
                        _dbContext.ManualTestCaseSteps.Remove(stepToRemove);
                    }
                }

                // === Handle Attributes update ===
                if (request.Attributes != null)
                {
                    var moduleAttributes = await _moduleAttributeRepository
                        .FindAsync(ma => ma.ModuleId == moduleId);

                    var existingAttributes = testCase.TestCaseAttributes.ToList();

                    foreach (var attrRequest in request.Attributes)
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
                                TestCaseId = testCase.Id,
                                ModuleAttributeId = moduleAttr.Id,
                                Value = attrRequest.Value
                            };
                            await _testCaseAttributeRepository.AddAsync(newAttr);
                        }
                    }

                    // Optional: Remove attributes not in the request if needed (not currently implemented)
                }

                _testCaseRepository.Update(testCase);
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

        public async Task<bool> DeleteTestCaseAsync(string moduleId, string id)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var testCase = await _dbContext.TestCases
                    .AsTracking()
                    .FirstOrDefaultAsync(tc => tc.ModuleId == moduleId && tc.Id == id);

                if (testCase == null) return false;

                // Delete related entities manually to avoid FK constraint errors

                var runResults = await _dbContext.TestRunResults
                    .Where(r => r.TestCaseId == id)
                    .ToListAsync();
                if (runResults.Any()) _dbContext.TestRunResults.RemoveRange(runResults);

                var suiteLinks = await _dbContext.TestSuiteTestCases
                    .Where(s => s.TestCaseId == id)
                    .ToListAsync();
                if (suiteLinks.Any()) _dbContext.TestSuiteTestCases.RemoveRange(suiteLinks);

                var steps = await _dbContext.ManualTestCaseSteps
                    .Where(s => s.TestCaseId == id)
                    .ToListAsync();
                if (steps.Any()) _dbContext.ManualTestCaseSteps.RemoveRange(steps);

                var attrs = await _dbContext.TestCaseAttributes
                    .Where(a => a.TestCaseId == id)
                    .ToListAsync();
                if (attrs.Any()) _dbContext.TestCaseAttributes.RemoveRange(attrs);

                var uploads = await _dbContext.Uploads
                    .Where(u => u.TestCaseId == id)
                    .ToListAsync();
                if (uploads.Any()) _dbContext.Uploads.RemoveRange(uploads);

                _dbContext.TestCases.Remove(testCase);

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
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

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

        // --- New methods for single attribute operations ---

        public async Task AddTestCaseAttributeAsync(string moduleId, string testCaseId, TestCaseAttributeRequest request)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
            if (testCase == null || testCase.ModuleId != moduleId)
                throw new KeyNotFoundException("Test case not found");

            var moduleAttribute = (await _moduleAttributeRepository.FindAsync(ma => ma.ModuleId == moduleId && ma.Key.ToLower() == request.Key.ToLower()))
                                  .FirstOrDefault();
            if (moduleAttribute == null)
                throw new KeyNotFoundException($"Module attribute with key '{request.Key}' not found");

            var existingAttr = (await _testCaseAttributeRepository.FindAsync(ta => ta.TestCaseId == testCaseId && ta.ModuleAttributeId == moduleAttribute.Id)).FirstOrDefault();
            if (existingAttr != null)
                throw new InvalidOperationException("Attribute already exists. Use update instead.");

            var newAttr = new TestCaseAttribute
            {
                TestCaseId = testCaseId,
                ModuleAttributeId = moduleAttribute.Id,
                Value = request.Value
            };

            await _testCaseAttributeRepository.AddAsync(newAttr);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateTestCaseAttributeAsync(string moduleId, string testCaseId, string key, TestCaseAttributeRequest request)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
            if (testCase == null || testCase.ModuleId != moduleId)
                return false;

            var attribute = (await _testCaseAttributeRepository.FindAsync(
                ta => ta.TestCaseId == testCaseId,
                include: q => q.Include(ta => ta.ModuleAttribute)))
                .FirstOrDefault(ta => string.Equals(ta.ModuleAttribute.Key, key, StringComparison.OrdinalIgnoreCase));

            if (attribute == null)
                return false;

            if (!string.Equals(key, request.Key, StringComparison.OrdinalIgnoreCase))
            {
                var newModuleAttribute = (await _moduleAttributeRepository
                    .FindAsync(ma => ma.ModuleId == moduleId && ma.Key.ToLower() == request.Key.ToLower()))
                    .FirstOrDefault();

                if (newModuleAttribute == null)
                    throw new KeyNotFoundException($"Module attribute with key '{request.Key}' not found");

                attribute.ModuleAttributeId = newModuleAttribute.Id;
            }

            attribute.Value = request.Value;
            _testCaseAttributeRepository.Update(attribute);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTestCaseAttributeAsync(string moduleId, string testCaseId, string key)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(testCaseId);
            if (testCase == null || testCase.ModuleId != moduleId)
                return false;

            var attribute = (await _testCaseAttributeRepository.FindAsync(
                ta => ta.TestCaseId == testCaseId,
                include: q => q.Include(ta => ta.ModuleAttribute)))
                .FirstOrDefault(ta => string.Equals(ta.ModuleAttribute.Key, key, StringComparison.OrdinalIgnoreCase));

            if (attribute == null)
                return false;

            // Instead of deleting record, clear the value
            attribute.Value = string.Empty;
            _testCaseAttributeRepository.Update(attribute);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
