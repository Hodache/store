namespace Store.DAL.DTO;

public class ProductDTO(string name)
{
    public int Id { get; }
    public string Name { get; } = name;

    public ProductDTO(int id, string name) : this(name)
    {
        Id = id;
    }
}
