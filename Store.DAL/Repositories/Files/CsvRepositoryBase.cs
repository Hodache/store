namespace Store.DAL.Repositories.Files;

public abstract class CsvRepositoryBase
{
    protected readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public CsvRepositoryBase(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Dispose();
        }
    }

    protected async Task<IEnumerable<string>> ReadAllLinesAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await File.ReadAllLinesAsync(_filePath);
        }
        finally
        {
            _lock.Release();
        }
    }

    protected async Task WriteAllLinesAsync(IEnumerable<string> lines)
    {
        await _lock.WaitAsync();
        try
        {
            await File.WriteAllLinesAsync(_filePath, lines);
        }
        finally
        {
            _lock.Release();
        }
    }

    protected async Task AppendLineAsync(string line)
    {
        await _lock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(_filePath, line + Environment.NewLine);
        }
        finally
        {
            _lock.Release();
        }
    }
}
