namespace Store.DAL.Entities;

public class ReserveEntity
{
    public int ShopId { get; set; }
    public ShopEntity Shop { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal Price { get; set; }
}