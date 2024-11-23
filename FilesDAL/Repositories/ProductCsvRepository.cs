using Domain.AbstractRepositories;
using Domain.DomainModel;

namespace FilesDAL.Repositories;

public class ProductCsvRepository : CsvRepositoryBase, IProductRepository
{
    public ProductCsvRepository(string filePath) : base(filePath) { }

    public async Task AddProductAsync(Product product)
    {
        product = new Product(GetNextIndex(), product.Name);

        var line = ToCsv(product);
        await AppendLineAsync(line);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(p => p.Id == id);
    }

    public async Task<Product?> GetProductByNameAsync(string name)
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

    private static Product ToDomain(string productLine)
    {
        var values = productLine.Split(',');
        return new Product(int.Parse(values[0]), values[1]);
    }

    private static string ToCsv(Product product)
    {
        return $"{product.Id},{product.Name}";
    }
}
