using Store.DAL.DTO;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Files;

public class ReserveCsvRepository : CsvRepositoryBase, IReserveRepository
{
    public ReserveCsvRepository(string filePath) : base(filePath) { }

    public async Task AddReserveAsync(ReserveDTO reserve)
    {
        var line = ToCsv(reserve);
        await AppendLineAsync(line);
    }

    public async Task DeleteReserveAsync(int shopId, int productId)
    {
        var reserves = (await GetAllReservesAsync()).Where(r => r.ShopId != shopId || r.ProductId != productId);
        await WriteAllLinesAsync(reserves.Select(ToCsv));
    }

    public async Task<IEnumerable<ReserveDTO>> GetAllReservesAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<ReserveDTO?> GetReserveAsync(int shopId, int productId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(r => r.ShopId == shopId && r.ProductId == productId);
    }

    public async Task<IEnumerable<ReserveDTO>> GetReservesByProductIdAsync(int productId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .Where(r => r.ProductId == productId);
    }

    public async Task<IEnumerable<ReserveDTO>> GetReservesByShopIdAsync(int shopId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .Where(r => r.ShopId == shopId);
    }

    public async Task UpdateReserveAsync(ReserveDTO reserve)
    {
        var reserves = (await GetAllReservesAsync()).ToList();
        var index = reserves.FindIndex(r => 
            r.ShopId == reserve.ShopId && 
            r.ProductId == reserve.ProductId);

        if (index >= 0)
        {
            reserves[index] = reserve;
            await WriteAllLinesAsync(reserves.Select(ToCsv));
        }
    }

    private static ReserveDTO ToDomain(string line)
    {
        var values = line.Split(',');
        return new ReserveDTO(
            int.Parse(values[0]), 
            int.Parse(values[1]), 
            int.Parse(values[2]), 
            decimal.Parse(values[3]));
    }

    private static string ToCsv(ReserveDTO reserve)
    {
        return $"{reserve.ShopId},{reserve.ProductId},{reserve.Quantity},{reserve.Price}";
    }
}
