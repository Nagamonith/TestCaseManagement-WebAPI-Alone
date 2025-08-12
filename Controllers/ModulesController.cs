using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Modules;
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/products/{productId}/modules")]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;
    private readonly ILogger<ModulesController> _logger;

    public ModulesController(
        IModuleService moduleService,
        ILogger<ModulesController> logger)
    {
        _moduleService = moduleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModuleResponse>>> GetAll(string productId)
    {
        try
        {
            var modules = await _moduleService.GetAllModulesAsync(productId);
            return Ok(modules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting modules for product {ProductId}", productId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving modules",
                details = ex.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModuleResponse>> GetById(string productId, string id)
    {
        try
        {
            var module = await _moduleService.GetModuleByIdAsync(productId, id);
            return module != null ? Ok(module) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting module {ModuleId} for product {ProductId}", id, productId);
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while retrieving module {id}",
                details = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string productId, [FromBody] CreateModuleRequest request)
    {
        try
        {
            // Validate productId format
            if (!Guid.TryParse(productId, out _))
            {
                return BadRequest(new { success = false, message = "Invalid Product ID format" });
            }

            request.ProductId = productId;
            var result = await _moduleService.CreateModuleAsync(request);
            return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error creating module");
            return StatusCode(500, new
            {
                success = false,
                message = "Database operation failed",
                details = ex.InnerException?.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            // e.g. product not found or no versions found
            _logger.LogWarning(ex, "Create module failed validation");
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating module for product {ProductId}", productId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while creating module",
                details = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string productId, string id, [FromBody] CreateModuleRequest request)
    {
        try
        {
            request.ProductId = productId;
            var success = await _moduleService.UpdateModuleAsync(id, request);
            return success ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Update module validation error for {ModuleId}", id);
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating module {ModuleId} for product {ProductId}", id, productId);
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while updating module {id}",
                details = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string productId, string id)
    {
        try
        {
            var success = await _moduleService.DeleteModuleAsync(productId, id);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting module {ModuleId} from product {ProductId}", id, productId);
            return StatusCode(500, new
            {
                success = false,
                message = $"An error occurred while deleting module {id}",
                details = ex.Message
            });
        }
    }
}
