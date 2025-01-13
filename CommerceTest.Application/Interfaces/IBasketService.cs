using CommerceTest.Domain.Models;
using CommerceTest.Domain.Requests;

namespace CommerceTest.Application.Interfaces;

public interface IBasketService
{
    public Task<string> AddItemToBasket(AddItemToBasketRequest request, CancellationToken ct = default);
    
    public Task<BasketDto> GetBasket(string id, CancellationToken ct = default);

    public Task<string> GenerateCheckoutLink(string basketId, CancellationToken ct = default);
}