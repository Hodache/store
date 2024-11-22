namespace Domain.DomainModel;

public class Reserve(Store store, Product product, int quantity, decimal price)
{
    public Store Store { get; set; } = store;
    public Product Product { get; set; } = product;
    public int Quantity { get; set; } = quantity;
    public decimal Price { get; set; } = price;
}
