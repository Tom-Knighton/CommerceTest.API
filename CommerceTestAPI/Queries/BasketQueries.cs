using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTestAPI.Queries;

[QueryType]
public static class BasketQueries
{
    public static async Task<BasketDto> GetBasket(string basketId, IBasketService basketService)
    {
        return await basketService.GetBasket(basketId);
    }
}