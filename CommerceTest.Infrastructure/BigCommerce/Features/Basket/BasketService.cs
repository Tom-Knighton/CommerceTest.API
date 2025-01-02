using System.Text.Json;
using BigCommerce.GraphQL;
using CommerceTest.Application.Interfaces;
using CommerceTest.Domain.Models;
using CommerceTest.Domain.Requests;
using Microsoft.AspNetCore.Http;

namespace CommerceTest.Infrastructure.BigCommerce.Features.Basket;

public class BigCommerceBasketService(IBigCommerceClient client, IHttpContextAccessor context, IMapper<IPhysicalItemFields, BasketItem> physicalMapper, IMapper<IDigitalItemFields, BasketItem> digitalMapper): IBasketService
{
    private const string CART_ITEM_WAREHOUSES_MAP_KEY = "CartItemWarehouses";
    
    public async Task<string> AddItemToBasket(AddItemToBasketRequest request, CancellationToken ct = default)
    {
        if (!int.TryParse(request.ProductId, out var productId))
        {
            throw new Exception("Invalid product Id");
        }

        var optionValues = request.Options.Select(x => new OptionValueId
        {
            OptionEntityId = int.Parse(x.OptionId),
            ValueEntityId = int.Parse(x.OptionValue)
        }).ToList();
        var variant = await client.GetVariantFromOptions.ExecuteAsync(productId, optionValues, ct);
        var variantId = variant.Data.Site.Product.Variants.Edges.FirstOrDefault().Node.EntityId;
        
        if (context.HttpContext.Request.Cookies.TryGetValue("BasketId", out var basketId) && !string.IsNullOrWhiteSpace(basketId))
        {
            var result = await client.AddItemToBasket.ExecuteAsync(basketId, productId, variantId, request.Quantity, ct);
            var cartId = result.Data.Cart.AddCartLineItems.Cart.EntityId;
            var cartMetadataResult = await client.GetCartWarehouseMap.ExecuteAsync(cartId, ct);
            var metafields = cartMetadataResult.Data.Site.Cart.Metafields.Edges.Select(x => x.Node).ToList();
            await UpdateWarehouseForCartItem(cartId, metafields, request.Warehouse,
                request.ProductId, variantId);
            await client.SaveCart.ExecuteAsync(cartId, ct);
            return cartId;
        }
        else
        {
            var createResponse = await client.CreateBasket.ExecuteAsync(productId, variantId, request.Quantity, ct);
            var cartId = createResponse.Data.Cart.CreateCart.Cart.EntityId;
            var cartMetadataResult = await client.GetCartWarehouseMap.ExecuteAsync(cartId, ct);
            var metafields = cartMetadataResult.Data.Site.Cart.Metafields.Edges.Select(x => x.Node).ToList();
            await UpdateWarehouseForCartItem(cartId, metafields, request.Warehouse,
                request.ProductId, variantId);
            await client.SaveCart.ExecuteAsync(cartId, ct);
            return cartId;
        }
    }

    public async Task<BasketDto> GetBasket(string id, CancellationToken ct = default)
    {
        var cartResponse = await client.GetBasket.ExecuteAsync(id, ct);
        var cart = cartResponse.Data.Site.Cart;

        var viewModel = new BasketDto()
        {
            BasketId = cart.EntityId,
            TotalItems = cart.LineItems.TotalQuantity
        };

        var items = new List<BasketItem>();
        
        items.AddRange(cart.LineItems.PhysicalItems.Select(physicalMapper.Map));
        items.AddRange(cart.LineItems.DigitalItems.Select(digitalMapper.Map));
        
        viewModel.Items = items;

        var metadata = cart.Metafields.Edges.Select(x => x.Node).ToList();
        var warehouseMapKVP = metadata.FirstOrDefault(x => x.Key == CART_ITEM_WAREHOUSES_MAP_KEY);
        ArgumentNullException.ThrowIfNull(warehouseMapKVP);
        var warehouseMap = JsonSerializer.Deserialize<Dictionary<string, string>>(warehouseMapKVP.Value);
        ArgumentNullException.ThrowIfNull(warehouseMap);
        
        foreach (var item in viewModel.Items)
        {
            var warehouseKVPKey = $"{item.ProductId}";
            if (item.VariantId is not null) warehouseKVPKey += $"-{item.VariantId}";
            if (!warehouseMap.TryGetValue(warehouseKVPKey, out var warehouse))
            {
                throw new Exception("SOMETHING WENT VERY BADLY WRONG HERE");
            }
            item.Warehouse = warehouse;
        }

        viewModel.SubTotal = viewModel.Items.Sum(i => i.Price * i.Quantity);

        return viewModel;
    }

    private async Task UpdateWarehouseForCartItem(string cartId, ICollection<IGetCartWarehouseMap_Site_Cart_Metafields_Edges_Node> metadata, string warehouse, string productId, int? variantId = null)
    {
        var key = $"{productId}";
        if (variantId is not null) key += $"-{variantId}";
        
        var existing = metadata.FirstOrDefault(m => m.Key == CART_ITEM_WAREHOUSES_MAP_KEY);
        if (existing != null)
        {
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(existing.Value);
            map[key] = warehouse;
            await client.UpdateCartWarehouseItemWarehouseMap.ExecuteAsync(cartId, existing.EntityId, JsonSerializer.Serialize(map));
        }
        else
        {
            var map = new Dictionary<string, string>()
            {
                { key, warehouse }
            };
            await client.AddCartItemWarehouseMap.ExecuteAsync(cartId, JsonSerializer.Serialize(map));
    
        }
    }
}