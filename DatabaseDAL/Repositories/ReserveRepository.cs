using Domain.AbstractRepositories;
using Domain.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace DatabaseDAL.Repositories;

public class ReserveRepository : IReserveRepository
{
    private readonly StoreDatabaseContext _context;

    public ReserveRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddReserveAsync(Reserve reserve)
    {
        var reserveEntity = ModelMapper.ToEntity(reserve);

        _context.Reserves.Add(reserveEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReserveAsync(int storeId, int productId)
    {
        var reserveEntity = await _context.Reserves.FindAsync(storeId, productId);

        if (reserveEntity != null)
        {
            _context.Reserves.Remove(reserveEntity);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Reserve?> GetReserveAsync(int storeId, int productId)
    {
        var reserveEntity = await _context.Reserves.FindAsync(storeId, productId);

        return reserveEntity == null ? null : ModelMapper.ToDomain(reserveEntity);
    }

    public async Task<IEnumerable<Reserve>> GetReservesByStoreIdAsync(int storeId)
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Store).Include(r => r.Product)
            .Where(r => r.StoreId == storeId)
            .ToListAsync();

        return reserveEntities.Select(ModelMapper.ToDomain);
    }

    public async Task<IEnumerable<Reserve>> GetReservesByProductIdAsync(int productId)
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Store).Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .ToListAsync();

        return reserveEntities.Select(ModelMapper.ToDomain);
    }

    public async Task<IEnumerable<Reserve>> GetAllReservesAsync()
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Store).Include(r => r.Product).ToListAsync();

        return reserveEntities.Select(ModelMapper.ToDomain);
    }

    public async Task UpdateReserveAsync(Reserve reserve)
    {
        var reserveEntity = await _context.Reserves.FindAsync(reserve.StoreId, reserve.ProductId);
        reserveEntity.Quantity = reserve.Quantity;
        reserveEntity.Price = reserve.Price;

        await _context.SaveChangesAsync();
    }
}
