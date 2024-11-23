using Domain.DomainModel;

namespace Domain.AbstractRepositories;

public interface IReserveRepository
{
    Task AddReserveAsync(Reserve reserve);
    Task<IEnumerable<Reserve>> GetReservesByStoreIdAsync(int storeId);
    Task<IEnumerable<Reserve>> GetReservesByProductIdAsync(int productId);
    Task<Reserve?> GetReserveAsync(int storeId, int productId);
    Task<IEnumerable<Reserve>> GetAllReservesAsync();
    Task UpdateReserveAsync(Reserve Reserve);
    Task DeleteReserveAsync(int storeId, int productId);
}
