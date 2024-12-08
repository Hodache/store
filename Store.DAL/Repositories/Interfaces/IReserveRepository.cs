using Store.DAL.DTO;

namespace Store.DAL.Repositories.Interfaces;

public interface IReserveRepository
{
    Task AddReserveAsync(ReserveDTO reserve);
    Task<IEnumerable<ReserveDTO>> GetReservesByShopIdAsync(int shopId);
    Task<IEnumerable<ReserveDTO>> GetReservesByProductIdAsync(int productId);
    Task<ReserveDTO?> GetReserveAsync(int shopId, int productId);
    Task<IEnumerable<ReserveDTO>> GetAllReservesAsync();
    Task UpdateReserveAsync(ReserveDTO reserve);
    Task DeleteReserveAsync(int shopId, int productId);
}
