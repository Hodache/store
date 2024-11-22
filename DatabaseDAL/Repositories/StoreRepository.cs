using Domain.AbstractRepositories;
using Domain.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace DatabaseDAL.Repositories;

public class StoreRepository : IStoreRepository
{
    private readonly StoreDatabaseContext _context;

    public StoreRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddStoreAsync(Store store)
    {
        var storeEntity = ModelMapper.ToEntity(store);

        _context.Stores.Add(storeEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Store>> GetAllStoresAsync()
    {
        var storeEntities = await _context.Stores.ToListAsync();
        return storeEntities.Select(ModelMapper.ToDomain);
    }

    public async Task<Store?> GetStoreByIdAsync(int id)
    {
        var storeEntity = await _context.Stores.FindAsync(id);
        return storeEntity == null ? null : ModelMapper.ToDomain(storeEntity);
    }
}
