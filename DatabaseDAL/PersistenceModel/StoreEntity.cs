namespace DatabaseDAL.PersistenceModel;

public class StoreEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public ICollection<ReserveEntity> Reserves { get; set; } = new List<ReserveEntity>();
}