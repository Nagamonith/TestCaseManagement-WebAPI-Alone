using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Api.Models.DTOs.Common;
using TestCaseManagement.Api.Models.DTOs.Products;
using TestCaseManagement.Api.Models.Responses.Products; // Add this namespace
using TestCaseManagement.Services.Interfaces;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/products/{productId}/versions")]
public class ProductVersionsController : ControllerBase
{
    private readonly IProductVersionService _productVersionService;

    public ProductVersionsController(IProductVersionService productVersionService)
    {
        _productVersionService = productVersionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductVersionResponse>>> GetAll(string productId)
    {
        var versions = await _productVersionService.GetAllVersionsAsync(productId);
        return Ok(versions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVersionResponse>> GetById(string productId, string id)
    {
        var version = await _productVersionService.GetVersionByIdAsync(productId, id);
        return version != null ? Ok(version) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create(string productId, [FromBody] ProductVersionRequest request)
    {
        var result = await _productVersionService.CreateVersionAsync(productId, request);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string productId, string id, [FromBody] ProductVersionRequest request)
    {
        var success = await _productVersionService.UpdateVersionAsync(productId, id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string productId, string id)
    {
        var success = await _productVersionService.DeleteVersionAsync(productId, id);
        return success ? NoContent() : NotFound();
    }
}