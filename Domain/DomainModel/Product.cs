namespace Domain.DomainModel;

public class Product(string name)
{
    public int Id { get; }
    public string Name { get; } = name;
}
