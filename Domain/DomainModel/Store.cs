namespace Domain.DomainModel;

public class Store(int id, string name, string address)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Address { get; } = address;
}