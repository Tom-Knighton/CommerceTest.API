using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Mappers;

public class BCCategoryMapper: IMapper<ICategoryTreeFields, Category>
{
    public Category Map(ICategoryTreeFields source)
    {
        return new Category
        {
            Id = source.EntityId.ToString(),
            Path = source.Path,
            CategoryName = source.Name
        };
    }
}