using AutoMapper;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Api.Models.Entities;
using TestCaseManagement.Repositories.Interfaces;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Services.Implementations;

public class TestRunResultService : ITestRunResultService
{
    private readonly IGenericRepository<TestRunResult> _repository;
    private readonly IGenericRepository<TestRun> _testRunRepository;
    private readonly IMapper _mapper;

    public TestRunResultService(
        IGenericRepository<TestRunResult> repository,
        IGenericRepository<TestRun> testRunRepository,
        IMapper mapper)
    {
        _repository = repository;
        _testRunRepository = testRunRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TestRunResultResponse>> GetAllResultsAsync(string testRunId)
    {
        var results = await _repository.FindAsync(r => r.TestRunId == testRunId);
        return _mapper.Map<IEnumerable<TestRunResultResponse>>(results);
    }

    public async Task RecordResultAsync(string testRunId, TestRunResultResponse request)
    {
        var testRun = await _testRunRepository.GetByIdAsync(testRunId);
        if (testRun == null) throw new KeyNotFoundException("Test run not found");

        var result = _mapper.Map<TestRunResult>(request);
        result.TestRunId = testRunId;
        result.ExecutedBy = "system"; // Replace with actual user from auth context

        await _repository.AddAsync(result);
    }
}