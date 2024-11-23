using Domain.AbstractRepositories;
using Domain.DomainModel;

namespace FilesDAL.Repositories;

public class StoreCsvRepository : CsvRepositoryBase, IStoreRepository
{
    public StoreCsvRepository(string filePath) : base(filePath) { }

    public async Task AddStoreAsync(Store store)
    {
        store = new Store(GetNextIndex(), store.Name, store.Address);

        var line = ToCsv(store);
        await AppendLineAsync(line);
    }

    public async Task<IEnumerable<Store>> GetAllStoresAsync()
    {
        var lines = await ReadAllLinesAsync();
        return lines.Select(ToDomain);
    }

    public async Task<Store?> GetStoreByIdAsync(int id)
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

    private static Store ToDomain(string storeLine)
    {
        var values = storeLine.Split(',');
        return new Store(int.Parse(values[0]), values[1], values[2]);
    }

    private static string ToCsv(Store store)
    {
        return $"{store.Id},{store.Name},{store.Address}";
    }
}
