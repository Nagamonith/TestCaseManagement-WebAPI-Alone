using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/modules/{moduleId}/attributes")]
public class ModuleAttributesController : ControllerBase
{
    private readonly IModuleAttributeService _attributeService;

    public ModuleAttributesController(IModuleAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleAttributeResponse>>> GetAll(string moduleId)
    {
        var attributes = await _attributeService.GetAllAttributesAsync(moduleId);
        return Ok(attributes);
    }


    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string moduleId, [FromBody] ModuleAttributeRequest request)
    {
        var result = await _attributeService.CreateAttributeAsync(moduleId, request);
        return CreatedAtAction(nameof(GetAll), new { moduleId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string moduleId, string id, [FromBody] ModuleAttributeRequest request)
    {
        var success = await _attributeService.UpdateAttributeAsync(moduleId, id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string moduleId, string id)
    {
        var success = await _attributeService.DeleteAttributeAsync(moduleId, id);
        return success ? NoContent() : NotFound();
    }
}
