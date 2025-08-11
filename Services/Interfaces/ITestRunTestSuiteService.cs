using TestCaseManagement.Api.Models.DTOs.TestRuns;

namespace TestCaseManagement.Services.Interfaces
{
    public interface ITestRunTestSuiteService
    {
        Task<IEnumerable<TestRunTestSuiteResponse>> GetAllTestSuitesAsync(string testRunId);
        Task AssignTestSuitesAsync(string testRunId, AssignTestSuitesRequest request);
        Task<bool> UpdateTestSuiteAsync(string testRunId, string testSuiteId, UpdateTestSuiteRequest request);
        Task<bool> RemoveTestSuiteAsync(string testRunId, string testSuiteId);
    }
}