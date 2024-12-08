using Microsoft.EntityFrameworkCore;
using Store.DAL.DTO;
using Store.DAL.Entities;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Database;

public class ReserveRepository : IReserveRepository
{
    private readonly StoreDatabaseContext _context;

    public ReserveRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddReserveAsync(ReserveDTO reserve)
    {
        var reserveEntity = Mapper.ToEntity(reserve);

        _context.Reserves.Add(reserveEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReserveAsync(int shopId, int productId)
    {
        var reserveEntity = await _context.Reserves.FindAsync(shopId, productId);

        if (reserveEntity != null)
        {
            _context.Reserves.Remove(reserveEntity);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ReserveDTO?> GetReserveAsync(int shopId, int productId)
    {
        var reserveEntity = await _context.Reserves.FindAsync(shopId, productId);

        return reserveEntity == null ? null : Mapper.ToDomain(reserveEntity);
    }

    public async Task<IEnumerable<ReserveDTO>> GetReservesByShopIdAsync(int shopId)
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Shop).Include(r => r.Product)
            .Where(r => r.ShopId == shopId)
            .ToListAsync();

        return reserveEntities.Select(Mapper.ToDomain);
    }

    public async Task<IEnumerable<ReserveDTO>> GetReservesByProductIdAsync(int productId)
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Shop).Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .ToListAsync();

        return reserveEntities.Select(Mapper.ToDomain);
    }

    public async Task<IEnumerable<ReserveDTO>> GetAllReservesAsync()
    {
        var reserveEntities = await _context.Reserves
            .Include(r => r.Shop).Include(r => r.Product).ToListAsync();

        return reserveEntities.Select(Mapper.ToDomain);
    }

    public async Task UpdateReserveAsync(ReserveDTO reserve)
    {
        var reserveEntity = await _context.Reserves.FindAsync(reserve.ShopId, reserve.ProductId);
        reserveEntity.Quantity = reserve.Quantity;
        reserveEntity.Price = reserve.Price;

        await _context.SaveChangesAsync();
    }
}
