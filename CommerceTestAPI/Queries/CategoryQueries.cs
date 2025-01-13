using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;
using HotChocolate.Types;

namespace CommerceTestAPI.Queries;

[QueryType]
public static class CategoryQueries
{
    public static async Task<ICollection<Category>> GetCategories(ICategoryService categoryService)
    {
        return await categoryService.GetL1Categories();
    }
}