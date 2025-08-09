using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestRuns;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestRunService
{
    Task<IEnumerable<TestRunResponse>> GetAllTestRunsAsync(string productId);
    Task<TestRunResponse?> GetTestRunByIdAsync(string productId, string id);
    Task<IdResponse> CreateTestRunAsync(string productId, CreateTestRunRequest request);
    Task<bool> UpdateTestRunStatusAsync(string productId, string id, string status);
    Task<bool> DeleteTestRunAsync(string productId, string id);
}