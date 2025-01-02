using AutoMapper;
using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Products;

public class BigCommerceProductService(IBigCommerceClient client, IMapper mapper): IProductService
{
    public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
    {
        var product = await client.ProductById.ExecuteAsync(productId, ct);
        return mapper.Map<Product>(product.Data.Site.Product);
    }

    public async Task<IEnumerable<Product>> GetProductsByQueryAsync(string query, int startFrom = 0, int limit = 20, CancellationToken ct = default)
    {
        var products = await client.QueryProducts.ExecuteAsync(query, ct);
        return products.Data.Site.Search.SearchProducts.Products.Edges.Select(e => mapper.Map<Product>(e.Node));
    }
}