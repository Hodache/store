using Microsoft.EntityFrameworkCore;
using Store.DAL.DTO;
using Store.DAL.Entities;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Database;

public class ShopRepository : IShopRepository
{
    private readonly StoreDatabaseContext _context;

    public ShopRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddShopAsync(ShopDTO shop)
    {
        var shopEntity = Mapper.ToEntity(shop);

        _context.Shops.Add(shopEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ShopDTO>> GetAllShopsAsync()
    {
        var shopEntities = await _context.Shops.ToListAsync();
        return shopEntities.Select(Mapper.ToDomain);
    }

    public async Task<ShopDTO?> GetShopByIdAsync(int id)
    {
        var shopEntity = await _context.Shops.FindAsync(id);
        return shopEntity == null ? null : Mapper.ToDomain(shopEntity);
    }
}
