using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;

namespace CommerceTest.Infrastructure.BigCommerce.Mappers;

public class BCPhysicalBasketItemMapper: IMapper<IPhysicalItemFields, BasketItem>
{
    public BasketItem Map(IPhysicalItemFields physicalItem)
    {
        var item = new BasketItem
        {
            Price = physicalItem.ListPrice.Value,
            Quantity = physicalItem.Quantity,
            ProductId = physicalItem.ProductEntityId.ToString(),
            VariantId = physicalItem.VariantEntityId.ToString(),
            ImageUrl = physicalItem.Image.Url960wide,
            ProductName = physicalItem.Name,
            BasketItemId = physicalItem.EntityId,
            SKU = physicalItem.Sku,
            SelectedOptions = []
        };

        foreach (var opt in physicalItem.SelectedOptions)
        {
            if (opt is IGetBasket_Site_Cart_LineItems_PhysicalItems_SelectedOptions_CartSelectedMultipleChoiceOption
                multi)
            {
                item.SelectedOptions.Add(new BasketItemOption
                {
                    OptionId = multi.EntityId.ToString(),
                    OptionValue = multi.Value,
                    OptionValueId = multi.ValueEntityId.ToString(),
                    OptionName = multi.Name,
                });
            }
        }

        return item;
    }
}

public class BCDigitalBasketItemMapper: IMapper<IDigitalItemFields, BasketItem>
{
    public BasketItem Map(IDigitalItemFields digitalItem)
    {
        var item = new BasketItem
        {
            Price = digitalItem.ListPrice.Value,
            Quantity = digitalItem.Quantity,
            ProductId = digitalItem.ProductEntityId.ToString(),
            VariantId = digitalItem.VariantEntityId.ToString(),
            ImageUrl = digitalItem.Image.Url960wide,
            ProductName = digitalItem.Name,
            BasketItemId = digitalItem.EntityId,
            SKU = digitalItem.Sku,
            SelectedOptions = []
        };

        return item;
    }
}