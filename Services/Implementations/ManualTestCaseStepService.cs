using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class ManualTestCaseStepService : IManualTestCaseStepService
{
    private readonly IGenericRepository<ManualTestCaseStep> _stepRepository;
    private readonly IGenericRepository<TestCase> _testCaseRepository;
    private readonly IMapper _mapper;

    public ManualTestCaseStepService(
        IGenericRepository<ManualTestCaseStep> stepRepository,
        IGenericRepository<TestCase> testCaseRepository,
        IMapper mapper)
    {
        _stepRepository = stepRepository;
        _testCaseRepository = testCaseRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ManualTestCaseStepRequest>> GetAllStepsAsync(string testCaseId)
    {
        var steps = await _stepRepository.FindAsync(s => s.TestCaseId == testCaseId);
        return _mapper.Map<IEnumerable<ManualTestCaseStepRequest>>(steps);
    }

    public async Task AddStepAsync(string testCaseId, ManualTestCaseStepRequest request)
    {
        var testCaseExists = await _testCaseRepository.GetByIdAsync(testCaseId) != null;
        if (!testCaseExists) throw new KeyNotFoundException("Test case not found");

        var step = _mapper.Map<ManualTestCaseStep>(request);
        step.TestCaseId = testCaseId;

        await _stepRepository.AddAsync(step);
        await _stepRepository.SaveChangesAsync(); // ✅ Persist to DB
    }

    public async Task<bool> UpdateStepAsync(string testCaseId, int stepId, ManualTestCaseStepRequest request)
    {
        var step = (await _stepRepository.FindAsync(s => s.TestCaseId == testCaseId && s.Id == stepId)).FirstOrDefault();
        if (step == null) return false;

        // Update the step properties
        step.Steps = request.Steps;
        step.ExpectedResult = request.ExpectedResult;

        _stepRepository.Update(step);
        await _stepRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStepAsync(string testCaseId, int stepId)
    {
        var step = (await _stepRepository.FindAsync(s => s.TestCaseId == testCaseId && s.Id == stepId)).FirstOrDefault();
        if (step == null) return false;

        _stepRepository.Remove(step);
        await _stepRepository.SaveChangesAsync(); // ✅ Persist removal
        return true;
    }

}