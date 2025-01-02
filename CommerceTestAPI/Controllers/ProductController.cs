using CommerceTest.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceTestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService): ControllerBase
{

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(int productId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await productService.GetProductByIdAsync(productId, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("search/{query}")]
    public async Task<IActionResult> GetProductById(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await productService.GetProductsByQueryAsync(query, 0, 25, cancellationToken));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}