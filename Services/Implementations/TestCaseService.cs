using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestCaseService : ITestCaseService
{
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IGenericRepository<Module> _moduleRepository;
    private readonly IMapper _mapper;

    public TestCaseService(
        IGenericRepository<TestCase> testCaseRepository,
        IGenericRepository<Module> moduleRepository,
        IMapper mapper)
    {
        _testCaseRepository = testCaseRepository;
        _moduleRepository = moduleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestCaseResponse>> GetAllTestCasesAsync(string moduleId)
    {
        var testCases = await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId);
        return _mapper.Map<IEnumerable<TestCaseResponse>>(testCases);
    }

    public async Task<TestCaseDetailResponse?> GetTestCaseByIdAsync(string moduleId, string id)
    {
        var testCase = await _testCaseRepository.FindAsync(tc => tc.ModuleId == moduleId && tc.Id == id);
        return _mapper.Map<TestCaseDetailResponse>(testCase.FirstOrDefault());
    }

    public async Task<IdResponse> CreateTestCaseAsync(CreateTestCaseRequest request)
    {
        var moduleExists = await _moduleRepository.GetByIdAsync(request.ModuleId) != null;
        if (!moduleExists) throw new KeyNotFoundException("Module not found");

        var testCase = _mapper.Map<TestCase>(request);
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
}