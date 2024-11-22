namespace Domain.DomainModel;

public class Store(string name, string address)
{
    public int Id { get; }
    public string Name { get; } = name;
    public string Address { get; } = address;
}