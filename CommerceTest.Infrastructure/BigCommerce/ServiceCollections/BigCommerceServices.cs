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
        });

        return services;
    }
}

public class BCDelegationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private void ConfigureCustomHeader(HttpRequestMessage request)
    {
        var customerHeader = httpContextAccessor.HttpContext?.Request.Headers["CustomerId"].FirstOrDefault();
        if (customerHeader is not null)
        {
            request.Headers.Add("X-Bc-Customer-Access-Token", customerHeader);
        }
        
        
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ConfigureCustomHeader(request);
        return await base.SendAsync(request, cancellationToken);
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ConfigureCustomHeader(request);
        return base.Send(request, cancellationToken);
    }
}
