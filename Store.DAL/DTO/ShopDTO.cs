namespace Store.DAL.DTO;

public class ShopDTO(string name, string address)
{
    public int Id { get; }
    public string Name { get; } = name;
    public string Address { get; } = address;

    public ShopDTO(int id, string name, string address) : this(name, address)
    {
        Id = id;
    }
}