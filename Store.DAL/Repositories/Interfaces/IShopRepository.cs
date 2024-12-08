using Store.DAL.DTO;

namespace Store.DAL.Repositories.Interfaces;

public interface IShopRepository
{
    Task AddShopAsync(ShopDTO shop);
    Task<ShopDTO?> GetShopByIdAsync(int id);
    Task<IEnumerable<ShopDTO>> GetAllShopsAsync();
}
