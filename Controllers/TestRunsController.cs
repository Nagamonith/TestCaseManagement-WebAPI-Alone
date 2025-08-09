using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestRuns;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/products/{productId}/testruns")]
public class TestRunsController : ControllerBase
{
    private readonly ITestRunService _testRunService;

    public TestRunsController(ITestRunService testRunService)
    {
        _testRunService = testRunService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestRunResponse>>> GetAll(string productId)
    {
        var testRuns = await _testRunService.GetAllTestRunsAsync(productId);
        return Ok(testRuns);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestRunResponse>> GetById(string productId, string id)
    {
        var testRun = await _testRunService.GetTestRunByIdAsync(productId, id);
        return testRun != null ? Ok(testRun) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string productId, [FromBody] CreateTestRunRequest request)
    {
        var result = await _testRunService.CreateTestRunAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string productId, string id, [FromBody] string status)
    {
        var success = await _testRunService.UpdateTestRunStatusAsync(productId, id, status);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string productId, string id)
    {
        var success = await _testRunService.DeleteTestRunAsync(productId, id);
        return success ? NoContent() : NotFound();
    }
}