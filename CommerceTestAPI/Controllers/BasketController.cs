using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace CommerceTestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketController(IBasketService basketService): ControllerBase
{
    [HttpPost("AddToBasket")]
    public async Task<IActionResult> AddItemToBasket(AddItemToBasketRequest request, CancellationToken ct = default)
    {
        var http = HttpContext.Request;
        var basketId = await basketService.AddItemToBasket(request, ct);
        
        HttpContext.Response.Cookies.Append("BasketId", basketId);
        
        return Ok(basketId);
    }

    [HttpGet("GetBasket")]
    public async Task<IActionResult> GetBasket(CancellationToken ct = default)
    {
        var id = HttpContext.Request.Cookies["BasketId"];
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest();
        }
        
        var basket = await basketService.GetBasket(id, ct);
        
        return Ok(basket);
    }
}