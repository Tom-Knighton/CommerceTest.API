using CommerceTest.Domain.Models;

namespace CommerceTest.Application.Interfaces;

public interface IProductService
{
    Task<Product?> GetProductByIdAsync(int productId, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetProductsByQueryAsync(string query, int startFrom = 0, int limit = 20, CancellationToken ct = default);
}