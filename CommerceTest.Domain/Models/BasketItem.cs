using System.Text.Json.Serialization;

namespace CommerceTest.Domain.Models;

public class BasketItem
{
    public string BasketItemId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public string? VariantId { get; set; }
    public decimal Price { get; set; }
    public string ProductName { get; set; }
    public string SKU { get; set; }
    public string ImageUrl { get; set; }
    public string Warehouse { get; set; }
    public string? Colour => SelectedOptions.FirstOrDefault(o => o.OptionName == "Color")?.OptionValue;
    public string? Size => SelectedOptions.FirstOrDefault(o => o.OptionName == "Size")?.OptionValue;

    [JsonIgnore] public ICollection<BasketItemOption> SelectedOptions { get; set; } = [];
}

public class BasketItemOption
{
    public string OptionId { get; set; }
    public string OptionName { get; set; }
    public string OptionValue { get; set; }
    public string OptionValueId { get; set; }
}