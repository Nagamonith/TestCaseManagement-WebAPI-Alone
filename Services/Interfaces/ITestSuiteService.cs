using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestSuites;

namespace TestCaseManagement.Services.Interfaces;

public interface ITestSuiteService
{
    Task<IEnumerable<TestSuiteResponse>> GetAllTestSuitesAsync(string productId);
    Task<TestSuiteResponse?> GetTestSuiteByIdAsync(string productId, string id);
    Task<IdResponse> CreateTestSuiteAsync(string productId, CreateTestSuiteRequest request);
    Task<bool> UpdateTestSuiteAsync(string productId, string id, CreateTestSuiteRequest request);
    Task<bool> DeleteTestSuiteAsync(string productId, string id, bool forceDelete = false);
}