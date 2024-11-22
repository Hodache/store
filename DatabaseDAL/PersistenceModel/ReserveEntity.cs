namespace DatabaseDAL.PersistenceModel;

internal class ReserveEntity
{
    public int StoreId { get; set; }
    public StoreEntity Store { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal Price { get; set; }
}