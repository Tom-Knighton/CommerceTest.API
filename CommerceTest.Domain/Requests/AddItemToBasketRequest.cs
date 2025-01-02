namespace CommerceTest.Domain.Requests;

public class AddItemToBasketRequest
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public ICollection<AddItemToBasketOption> Options { get; set; } = [];
    public string Warehouse { get; set; }
}

public class AddItemToBasketOption
{
    public string OptionId { get; set; }
    public string OptionValue { get; set; }
}