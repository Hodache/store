using Store.DAL.DTO;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Files;

public class ShopCsvRepository : CsvRepositoryBase, IShopRepository
{
    public ShopCsvRepository(string filePath) : base(filePath) { }

    public async Task AddShopAsync(ShopDTO shop)
    {
        shop = new ShopDTO(GetNextIndex(), shop.Name, shop.Address);

        var line = ToCsv(shop);
        await AppendLineAsync(line);
    }

    public async Task<IEnumerable<ShopDTO>> GetAllShopsAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<ShopDTO?> GetShopByIdAsync(int id)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(s => s.Id == id);
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

    private static ShopDTO ToDomain(string shopLine)
    {
        var values = shopLine.Split(',');
        return new ShopDTO(int.Parse(values[0]), values[1], values[2]);
    }

    private static string ToCsv(ShopDTO shop)
    {
        return $"{shop.Id},{shop.Name},{shop.Address}";
    }
}
