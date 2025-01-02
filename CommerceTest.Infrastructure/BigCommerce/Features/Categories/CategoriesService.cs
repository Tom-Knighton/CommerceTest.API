using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Features.Categories;

public class BigCommerceCategoriesService(IBigCommerceClient client, IMapper<ICategoryTreeFields, Category> mapper): ICategoryService
{
    public async Task<Category[]> GetL1Categories(CancellationToken ct = default)
    {
        var categories = await client.GetRootCategories.ExecuteAsync(ct);

        return categories.Data.Site.CategoryTree.Select(mapper.Map).ToArray();
    }
}