using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/modules/{moduleId}/testcases")]
public class TestCasesController : ControllerBase
{
    private readonly ITestCaseService _testCaseService;

    public TestCasesController(ITestCaseService testCaseService)
    {
        _testCaseService = testCaseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestCaseResponse>>> GetAll(string moduleId)
    {
        var testCases = await _testCaseService.GetAllTestCasesAsync(moduleId);
        return Ok(testCases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestCaseDetailResponse>> GetById(string moduleId, string id)
    {
        var testCase = await _testCaseService.GetTestCaseByIdAsync(moduleId, id);
        return testCase != null ? Ok(testCase) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string moduleId, [FromBody] CreateTestCaseRequest request)
    {
        request.ModuleId = moduleId;
        var result = await _testCaseService.CreateTestCaseAsync(request);
        return CreatedAtAction(nameof(GetById), new { moduleId, id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string moduleId, string id, [FromBody] UpdateTestCaseRequest request)
    {
        var success = await _testCaseService.UpdateTestCaseAsync(moduleId, id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string moduleId, string id)
    {
        var success = await _testCaseService.DeleteTestCaseAsync(moduleId, id);
        return success ? NoContent() : NotFound();
    }
}