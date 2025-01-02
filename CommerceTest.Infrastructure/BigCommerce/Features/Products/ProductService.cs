using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Products;

public class BigCommerceProductService(IBigCommerceClient client, IMapper<IProductFields, Product> mapper): IProductService
{
    public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default)
    {
        var productResult = await client.ProductById.ExecuteAsync(productId, ct);
        var product = productResult.Data.Site.Product;
        return mapper.Map(product);
    }

    public async Task<IEnumerable<Product>> GetProductsByQueryAsync(string query, int startFrom = 0, int limit = 20, CancellationToken ct = default)
    {
        var products = await client.QueryProducts.ExecuteAsync(query, ct);
        return products.Data.Site.Search.SearchProducts.Products.Edges.Select(e => mapper.Map(e.Node));
    }
}