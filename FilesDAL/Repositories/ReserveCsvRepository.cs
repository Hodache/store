using Domain.AbstractRepositories;
using Domain.DomainModel;

namespace FilesDAL.Repositories;

public class ReserveCsvRepository : CsvRepositoryBase, IReserveRepository
{
    public ReserveCsvRepository(string filePath) : base(filePath) { }

    public async Task AddReserveAsync(Reserve reserve)
    {
        var line = ToCsv(reserve);
        await AppendLineAsync(line);
    }

    public async Task DeleteReserveAsync(int storeId, int productId)
    {
        var reserves = (await GetAllReservesAsync()).Where(r => r.StoreId != storeId || r.ProductId != productId);
        await WriteAllLinesAsync(reserves.Select(ToCsv));
    }

    public async Task<IEnumerable<Reserve>> GetAllReservesAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<Reserve?> GetReserveAsync(int storeId, int productId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .FirstOrDefault(r => r.StoreId == storeId && r.ProductId == productId);
    }

    public async Task<IEnumerable<Reserve>> GetReservesByProductIdAsync(int productId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .Where(r => r.ProductId == productId);
    }

    public async Task<IEnumerable<Reserve>> GetReservesByStoreIdAsync(int storeId)
    {
        var lines = await ReadAllLinesAsync();
        return lines
            .Select(ToDomain)
            .Where(r => r.StoreId == storeId);
    }

    public async Task UpdateReserveAsync(Reserve reserve)
    {
        var reserves = (await GetAllReservesAsync()).ToList();
        var index = reserves.FindIndex(r => 
            r.StoreId == reserve.StoreId && 
            r.ProductId == reserve.ProductId);

        if (index >= 0)
        {
            reserves[index] = reserve;
            await WriteAllLinesAsync(reserves.Select(ToCsv));
        }
    }

    private static Reserve ToDomain(string line)
    {
        var values = line.Split(',');
        return new Reserve(
            int.Parse(values[0]), 
            int.Parse(values[1]), 
            int.Parse(values[2]), 
            decimal.Parse(values[3]));
    }

    private static string ToCsv(Reserve reserve)
    {
        return $"{reserve.StoreId},{reserve.ProductId},{reserve.Quantity},{reserve.Price}";
    }
}
