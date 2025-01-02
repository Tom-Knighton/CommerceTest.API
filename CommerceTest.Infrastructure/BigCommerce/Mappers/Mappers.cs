using AutoMapper;
using BigCommerce.GraphQL;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Mappers;

public class BigCommerceMapProfile : Profile
{
    public BigCommerceMapProfile()
    {
        CreateMap<IProductById_Site_Product_ReviewSummary, ProductRatingSummary>()
            .ForMember(dest => dest.Average, opt => opt.MapFrom(src => src.SummationOfRatings))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.NumberOfReviews));
        
        CreateMap<IProductFields, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.PlainTextDescription))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Prices.Price.Value))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Edges.Select(x => x.Node.Url960wide)))
            .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand.Name))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.ProductOptions.Edges.Select(x => x.Node)))
            .ForMember(dest => dest.RatingSummary, opt =>
            {
                opt.PreCondition(src => src.ReviewSummary is not null);
                opt.MapFrom(src => src.ReviewSummary);
            });

        CreateMap<IProductById_Site_Product_ProductOptions_Edges_Node, ProductOption>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
            .ForMember(dest => dest.Values, opt => opt.Ignore());

        CreateMap<IProductById_Site_Product_ProductOptions_Edges_Node_MultipleChoiceOption, ProductOption>()
            .IncludeBase<IProductById_Site_Product_ProductOptions_Edges_Node, ProductOption>()
            .ForMember(dest => dest.Values, opt => opt.MapFrom(src => src.Values.Edges.Select(x => x.Node)));

        CreateMap<IProductById_Site_Product_ProductOptions_Edges_Node_Values_Edges_Node, ProductOptionValue>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Label));
        // .ForMember(dest => dest.Hex, opt => opt.MapFrom(src => src.))

       
        CreateMap<IProductById_Site_Product, Product>()
            .IncludeBase<IProductFields, Product>();

        CreateMap<IQueryProducts_Site_Search_SearchProducts_Products_Edges_Node, Product>()
            .IncludeBase<IProductFields, Product>();

        CreateMap<IGetRootCategories_Site_CategoryTree_CategoryTreeItem, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EntityId.ToString()))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name));

        CreateMap<ICartFields, BasketDetails>()
            .ForMember(dest => dest.BasketId, opt => opt.MapFrom(src => src.EntityId));
            

        CreateMap<IGetBasket_Site_Cart_LineItems_PhysicalItems, BasketItem>()
            .ForMember(dest => dest.BasketItemId, opt => opt.MapFrom(src => src.EntityId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductEntityId))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ListPrice.Value))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => src.VariantEntityId))
            .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.Sku))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image.Url960wide))
            .ForMember(dest => dest.SelectedOptions, opt => opt.MapFrom(src => src.SelectedOptions));

        CreateMap<IGetBasket_Site_Cart_LineItems_PhysicalItems_SelectedOptions, BasketItemOption>()
            .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.EntityId.ToString()));
        
        CreateMap<IGetBasket_Site_Cart_LineItems_PhysicalItems_SelectedOptions_CartSelectedMultipleChoiceOption, BasketItemOption>()
            .IncludeBase<IGetBasket_Site_Cart_LineItems_PhysicalItems_SelectedOptions, BasketItemOption>()
            .ForMember(dest => dest.OptionValue, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.OptionValueId, opt => opt.MapFrom(src => src.ValueEntityId.ToString()));

        CreateMap<IGetBasket_Site_Cart_LineItems_DigitalItems_SelectedOptions, BasketItemOption>()
            .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.Name));
        
    }
}