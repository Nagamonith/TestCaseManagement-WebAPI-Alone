using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.TestCases;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/testcases/{testCaseId}/attributes")]
public class TestCaseAttributesController : ControllerBase
{
    private readonly ITestCaseAttributeService _attributeService;

    public TestCaseAttributesController(ITestCaseAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestCaseAttributeResponse>>> GetAll(string testCaseId)
    {
        var attributes = await _attributeService.GetAllAttributesAsync(testCaseId);
        return Ok(attributes);
    }

    [HttpPost]
    public async Task<ActionResult> Create(string testCaseId, [FromBody] TestCaseAttributeRequest request)
    {
        await _attributeService.AddAttributeAsync(testCaseId, request);
        return NoContent();
    }
    [HttpPut("{key}")]
    public async Task<ActionResult> Update(string testCaseId, string key, [FromBody] TestCaseAttributeRequest request)
    {
        var success = await _attributeService.UpdateAttributeAsync(testCaseId, key, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string testCaseId, string key)
    {
        var success = await _attributeService.DeleteAttributeAsync(testCaseId, key);
        return success ? NoContent() : NotFound();
    }
}