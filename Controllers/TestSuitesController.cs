using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.TestSuites;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/products/{productId}/testsuites")]
public class TestSuitesController : ControllerBase
{
    private readonly ITestSuiteService _testSuiteService;

    public TestSuitesController(ITestSuiteService testSuiteService)
    {
        _testSuiteService = testSuiteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestSuiteResponse>>> GetAll(string productId)
    {
        var testSuites = await _testSuiteService.GetAllTestSuitesAsync(productId);
        return Ok(testSuites);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestSuiteResponse>> GetById(string productId, string id)
    {
        var testSuite = await _testSuiteService.GetTestSuiteByIdAsync(productId, id);
        return testSuite != null ? Ok(testSuite) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string productId, [FromBody] CreateTestSuiteRequest request)
    {
        var result = await _testSuiteService.CreateTestSuiteAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string productId, string id, [FromBody] CreateTestSuiteRequest request)
    {
        var success = await _testSuiteService.UpdateTestSuiteAsync(productId, id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string productId, string id)
    {
        var success = await _testSuiteService.DeleteTestSuiteAsync(productId, id);
        return success ? NoContent() : NotFound();
    }
}