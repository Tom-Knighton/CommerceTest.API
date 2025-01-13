using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTestAPI.Queries;

[QueryType]
public static class ProductQueries
{
    public static async Task<Product?> GetProduct(int productId, IProductService productService)
    {
        return await productService.GetProductByIdAsync(productId);
    }
}