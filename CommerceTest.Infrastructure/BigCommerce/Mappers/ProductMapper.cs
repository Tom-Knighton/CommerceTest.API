using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Mappers;

public class BCProductMapper: IMapper<IProductFields, Product>
{
    public Product Map(IProductFields product)
    {
        var dto = new Product
        {
            Id = product.EntityId.ToString(),
            Brand = product.Brand?.Name ?? "Mountain Warehouse",
            Name = product.Name,
            Description = product.PlainTextDescription,
            Images = product.Images.Edges.Select(x => x.Node.Url960wide).ToList(),
            Price = product.Prices.Price.Value,
            RatingSummary = new ProductRatingSummary
            {
                Average = product.ReviewSummary.SummationOfRatings,
                Count = product.ReviewSummary.NumberOfReviews
            },
            Options = []
        };

        foreach (var opt in product.ProductOptions.Edges)
        {
            var option = opt.Node;
            if (option is IProductById_Site_Product_ProductOptions_Edges_Node_MultipleChoiceOption multi)
            {
                dto.Options.Add(new ProductOption
                {
                    Id = multi.EntityId.ToString(),
                    Name = multi.DisplayName,
                    Values = multi.Values.Edges.Select(x => new ProductOptionValue
                    {
                        Id = x.Node.EntityId.ToString(),
                        Name = x.Node.Label
                    }).ToList()
                });
            }
        }

        return dto;
    }
}