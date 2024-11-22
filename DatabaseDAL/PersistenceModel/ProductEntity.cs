namespace DatabaseDAL.PersistenceModel;

internal class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<ReserveEntity> Reserves { get; set; } = new List<ReserveEntity>();
}