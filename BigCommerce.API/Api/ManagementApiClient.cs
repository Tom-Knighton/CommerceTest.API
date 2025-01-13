using System.Net.Http.Json;
using BigCommerce.API.Models.CartsV3;
using CommerceTest.Domain.Requests;

namespace BigCommerce.API.Api;

public interface IBCManagementApiClient
{
    
}

public class BCManagementApiClient(HttpClient httpClient) : IBCManagementApiClient
{

    public async Task<Cart_Full> CreateCart(AddItemToBasketRequest request, CancellationToken ct = default)
    {
        var createData = new CartCreatePostData()
        {
            Customer_id = 0,
            Channel_id = 1689596,
            Currency = new Currency2 { Code = "GBP" },
            Line_items =
            [

            ]
        };


        var response = await httpClient.PostAsJsonAsync("v3/carts", new
        {

        }, cancellationToken: ct);

        response.EnsureSuccessStatusCode();

        return null;
    }
}