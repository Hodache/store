using Store.DAL.DTO;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Files;

public class ProductCsvRepository : CsvRepositoryBase, IProductRepository
{
    public ProductCsvRepository(string filePath) : base(filePath) { }

    public async Task AddProductAsync(ProductDTO product)
    {
        product = new ProductDTO(GetNextIndex(), product.Name);

        var line = ToCsv(product);
        await AppendLineAsync(line);
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int id)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(p => p.Id == id);
    }

    public async Task<ProductDTO?> GetProductByNameAsync(string name)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(p => p.Name == name);
    }

    private int GetNextIndex()
    {
        var maxIndex = 0;
        var lines = File.ReadAllLines(_filePath);

        if (lines.Length != 0)
        {
            maxIndex = lines
            .Select(line => int.Parse(line.Split(',')[0]))
            .Max();
        }

        return maxIndex + 1;
    }

    private static ProductDTO ToDomain(string productLine)
    {
        var values = productLine.Split(',');
        return new ProductDTO(int.Parse(values[0]), values[1]);
    }

    private static string ToCsv(ProductDTO product)
    {
        return $"{product.Id},{product.Name}";
    }
}
