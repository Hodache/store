namespace Domain.DomainModel;

public class Reserve(Store store, Product product, int quantity, decimal price)
{
    public Store Store { get; } = store;
    public Product Product { get; } = product;
    public int Quantity { get; } = quantity;
    public decimal Price { get; } = price;
}
