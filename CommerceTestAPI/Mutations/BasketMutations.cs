using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Requests;
using StrawberryShake;

namespace CommerceTestAPI.Mutations;

public class Mutation
{
    public BasketMutations GetBasket() => new();
}

public class BasketMutations
{
    public Task<string> AddItemToBasket(AddItemToBasketRequest request, [Service] IBasketService basketService)
    {
        return basketService.AddItemToBasket(request);
    }

    public Task<string> GenerateCheckoutLink([Service] IBasketService basketService, [Service] IHttpContextAccessor httpContextAccessor, CancellationToken ct = default)
    {
        var basketId = httpContextAccessor.HttpContext?.Request.Cookies["BasketId"];
        if (string.IsNullOrWhiteSpace(basketId))
        {
            throw new GraphQLClientException("Missing Basket Id");
        }
        
        return basketService.GenerateCheckoutLink(basketId, ct);
    }
}