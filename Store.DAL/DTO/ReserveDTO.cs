namespace Store.DAL.DTO;

public class ReserveDTO(int shopId, int productId, int quantity, decimal price)
{
    public int ShopId { get; set; } = shopId;
    public int ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;
    public decimal Price { get; set; } = price;
}
