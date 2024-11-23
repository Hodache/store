namespace Domain.DomainModel;

public class Reserve(int storeId, int productId, int quantity, decimal price)
{
    public int StoreId { get; set; } = storeId;
    public int ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
    public decimal Price { get; set; } = price;
}
