using TestCaseManagement.Api.Models.DTOs.TestRuns;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestRunTestSuiteService
{
    Task AssignTestSuitesAsync(string testRunId, AssignTestSuitesRequest request);
    Task<bool> RemoveTestSuiteAsync(string testRunId, string testSuiteId);
}