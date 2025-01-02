namespace CommerceTest.Domain.Models;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public string? Brand { get; set; } = "Mountain Warehouse";

    public ICollection<string> Images { get; set; } = [];

    public ICollection<ProductOption> Options { get; set; } = [];
    
    public ProductRatingSummary? RatingSummary { get; set; }
}

public class ProductOption
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsRequired { get; set; }

    public ICollection<ProductOptionValue> Values { get; set; } = [];
}

public class ProductOptionValue
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Hex { get; set; }
    public string? Image { get; set; }
}

public class ProductRatingSummary
{
    public double Average { get; set; }
    public int Count { get; set; }
}