using TestCaseManagement.Api.Models.DTOs.TestRuns;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestRunResultService
{
    Task<IEnumerable<TestRunResultResponse>> GetAllResultsAsync(string testRunId);
    Task RecordResultAsync(string testRunId, TestRunResultResponse request);
}