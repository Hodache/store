namespace Store.BLL.DTO;

public class ReserveDto
{
    public int ShopId { get; set; }
    public string ShopName { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
