using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testruns/{testRunId}/results")]
public class TestRunResultsController : ControllerBase
{
    private readonly ITestRunResultService _service;

    public TestRunResultsController(ITestRunResultService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestRunResultResponse>>> GetAll(string testRunId)
    {
        var results = await _service.GetAllResultsAsync(testRunId);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult> Record(string testRunId, [FromBody] TestRunResultResponse request)
    {
        await _service.RecordResultAsync(testRunId, request);
        return NoContent();
    }
}