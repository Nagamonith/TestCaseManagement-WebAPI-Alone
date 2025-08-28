using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Services.Interfaces;
using TestCaseManagementService.Models.DTOs.TestCases;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testcases/{testCaseId}/steps")]
public class ManualTestCaseStepsController : ControllerBase
{
    private readonly IManualTestCaseStepService _stepService;

    public ManualTestCaseStepsController(IManualTestCaseStepService stepService)
    {
        _stepService = stepService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManualTestCaseStepRequest>>> GetAll(string testCaseId)
    {
        var steps = await _stepService.GetAllStepsAsync(testCaseId);
        return Ok(steps);
    }

    [HttpPost]
    public async Task<ActionResult> Create(string testCaseId, [FromBody] ManualTestCaseStepRequest request)
    {
        await _stepService.AddStepAsync(testCaseId, request);
        return NoContent();
    }

    [HttpPut("{stepId}")]
    public async Task<ActionResult> Update(string testCaseId, int stepId, [FromBody] ManualTestCaseStepRequest request)
    {
        var success = await _stepService.UpdateStepAsync(testCaseId, stepId, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{stepId}")]
    public async Task<ActionResult> Delete(string testCaseId, int stepId)
    {
        var success = await _stepService.DeleteStepAsync(testCaseId, stepId);
        return success ? NoContent() : NotFound();
    }
}