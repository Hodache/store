namespace Store.DAL.Entities;

public class ShopEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<ReserveEntity> Reserves { get; set; } = new List<ReserveEntity>();
}