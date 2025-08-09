using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testruns/{testRunId}/testsuites")]
public class TestRunTestSuitesController : ControllerBase
{
    private readonly ITestRunTestSuiteService _service;

    public TestRunTestSuitesController(ITestRunTestSuiteService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Assign(string testRunId, [FromBody] AssignTestSuitesRequest request)
    {
        await _service.AssignTestSuitesAsync(testRunId, request);
        return NoContent();
    }

    [HttpDelete("{testSuiteId}")]
    public async Task<ActionResult> Remove(string testRunId, string testSuiteId)
    {
        var success = await _service.RemoveTestSuiteAsync(testRunId, testSuiteId);
        return success ? NoContent() : NotFound();
    }
}