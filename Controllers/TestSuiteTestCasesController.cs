using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testsuites/{testSuiteId}/testcases")]
public class TestSuiteTestCasesController : ControllerBase
{
    private readonly ITestSuiteTestCaseService _service;

    public TestSuiteTestCasesController(ITestSuiteTestCaseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<TestSuiteWithCasesResponse>> GetAll(string testSuiteId)
    {
        var result = await _service.GetAllTestCasesAsync(testSuiteId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Assign(string testSuiteId, [FromBody] AssignTestCasesRequest request)
    {
        await _service.AssignTestCasesAsync(testSuiteId, request);
        return NoContent();
    }

    [HttpDelete("{testCaseId}")]
    public async Task<ActionResult> Remove(string testSuiteId, string testCaseId)
    {
        var success = await _service.RemoveTestCaseAsync(testSuiteId, testCaseId);
        return success ? NoContent() : NotFound();
    }
}