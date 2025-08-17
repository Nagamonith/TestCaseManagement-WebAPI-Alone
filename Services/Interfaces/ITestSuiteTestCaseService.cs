using TestCaseManagement.Api.Models.DTOs.TestSuites;

namespace TestCaseManagement.Services.Interfaces
{
    public interface ITestSuiteTestCaseService
    {
        Task<TestSuiteWithCasesResponse> GetAllTestCasesAsync(string testSuiteId);
        Task<TestSuiteTestCaseResponse> GetTestCaseExecutionDetailsAsync(int testSuiteTestCaseId);
        Task AssignTestCasesAsync(string testSuiteId, AssignTestCasesRequest request);
        Task<bool> RemoveTestCaseAsync(string testSuiteId, string testCaseId);
        Task RemoveAllAssignmentsAsync(string testSuiteId);

        // New methods for execution details
        Task UpdateExecutionDetailsAsync(int testSuiteTestCaseId, UpdateExecutionDetailsRequest request);

        // New methods for uploads
        Task AddExecutionUploadAsync(int testSuiteTestCaseId, AddExecutionUploadRequest request);
        Task RemoveExecutionUploadAsync(string uploadId);
    }
}