using TestCaseManagement.Api.Models.DTOs.TestSuites;

namespace TestCaseManagement.Services.Interfaces
{
    public interface ITestSuiteTestCaseService
    {
        Task<TestSuiteWithCasesResponse> GetAllTestCasesAsync(string testSuiteId);
        Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request);
        Task<bool> RemoveTestCaseAsync(string testSuiteId, string testCaseId);

        // NEW method for removing all test case assignments from a test suite
        Task RemoveAllAssignmentsAsync(string testSuiteId);
    }
}
