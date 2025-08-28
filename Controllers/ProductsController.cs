using Microsoft.AspNetCore.Mvc;
using TestCaseManagement.Services.Interfaces;
using TestCaseManagementService.Models.DTOs.Common;
using TestCaseManagementService.Models.DTOs.Products;

namespace TestCaseManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetById(string id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return product != null ? Ok(product) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<IdResponse>> Create([FromBody] CreateProductRequest request)
    {
        var result = await _productService.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
    {
        var success = await _productService.UpdateProductAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _productService.DeleteProductAsync(id);
        return success ? NoContent() : NotFound();
    }
}