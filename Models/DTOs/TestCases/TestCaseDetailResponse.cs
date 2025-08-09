using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Api.Models.DTOs.Uploads;

namespace TestCaseManagement.Api.Models.DTOs.TestCases;

public class TestCaseDetailResponse : TestCaseResponse
{
    public List<ManualTestCaseStepRequest> Steps { get; set; } = new();
    public List<TestCaseAttributeResponse> Attributes { get; set; } = new();
    public List<UploadResponse> Attachments { get; set; } = new();
    public List<string> TestSuiteIds { get; set; } = new();
}