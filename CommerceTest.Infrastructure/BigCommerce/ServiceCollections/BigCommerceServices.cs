using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;
using CommerceTest.Infrastructure.BigCommerce.Features.Basket;
using CommerceTest.Infrastructure.BigCommerce.Features.Categories;
using CommerceTest.Infrastructure.BigCommerce.Mappers;
using CommerceTest.Infrastructure.BigCommerce.Products;
using CommerceTest.Infrastructure.BigCommerce.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommerceTest.Infrastructure.BigCommerce.ServiceCollections;

public static class BigCommerceServices
{
    public static IServiceCollection AddBigCommerceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<BCDelegationHandler>();
        services.AddTransient<IProductService, BigCommerceProductService>();
        services.AddTransient<ICategoryService, BigCommerceCategoriesService>();
        services.AddTransient<IBasketService, BigCommerceBasketService>();

        services.AddTransient<IMapper<IProductFields, Product>, BCProductMapper>();
        services.AddTransient<IMapper<IPhysicalItemFields, BasketItem>, BCPhysicalBasketItemMapper>();
        services.AddTransient<IMapper<IDigitalItemFields, BasketItem>, BCDigitalBasketItemMapper>();
        services.AddTransient<IMapper<ICategoryTreeFields, Category>, BCCategoryMapper>();
        
        services.AddSerializer<BigDecimalSerializer>();
        services
            .AddBigCommerceClient()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(configuration["BaseAddress"]);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + configuration["AuthToken"]);
            }, builder => builder.AddHttpMessageHandler<BCDelegationHandler>());

        services.AddHttpClient("BigCommerce", client =>
        {
            client.BaseAddress = new Uri("https://api.bigcommerce.com/stores/");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + configuration["AuthToken"]);
        }).AddHttpMessageHandler<BCDelegationHandler>();

        return services;
    }
}

public class BCDelegationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private string[] ProxyCookies = ["SHOP_SESSION_TOKEN", "__HOST-SHOP_SESSION_TOKEN", "athena_short_visit_id", "fornax_anonymousId"];
    private void ConfigureCustomHeader(HttpRequestMessage request)
    {
        var customerHeader = httpContextAccessor.HttpContext?.Request.Headers["CustomerId"].FirstOrDefault();
        if (customerHeader is not null)
        {
            request.Headers.Add("X-Bc-Customer-Id", customerHeader);
        }

        var reqCookies = httpContextAccessor.HttpContext?.Request.Headers.Cookie;
        if (!string.IsNullOrWhiteSpace(reqCookies))
        {
            request.Headers.Add("Cookie", reqCookies.ToString());
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ConfigureCustomHeader(request);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
        {
            foreach (var setCookie in setCookieHeaders)
            {
                httpContextAccessor.HttpContext?.Response.Headers.Append("Set-Cookie", setCookie);
            }
        }
        
        return response;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ConfigureCustomHeader(request);
        return base.Send(request, cancellationToken);
    }
}
