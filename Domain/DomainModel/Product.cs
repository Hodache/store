namespace Domain.DomainModel;

public class Product(string name)
{
    public int Id { get; }
    public string Name { get; } = name;

    public Product(int id, string name) : this(name)
    {
        Id = id;
    }
}
