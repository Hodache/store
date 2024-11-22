namespace Domain.DomainModel;

public class Product(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
