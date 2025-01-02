using CommerceTest.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommerceTestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController(ICategoryService categoryService): ControllerBase
{
    [HttpGet("L1")]
    public async Task<IActionResult> GetL1Categories()
    {
        var categories = await categoryService.GetL1Categories();
        return Ok(categories);
    }
}