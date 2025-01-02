namespace CommerceTest.Domain.Models;

public class BasketDetails
{
    public string BasketId { get; set; }
}

public class BasketDto
{
    public string BasketId { get; set; }
    public int TotalItems { get; set; }
    public ICollection<BasketItem> Items { get; set; } = [];
    public decimal SubTotal { get; set; }
}