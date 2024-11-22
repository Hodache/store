using Domain.DomainModel;

namespace Domain.AbstractRepositories;

public interface IReserveRepository
{
    Task AddReserveAsync(Reserve reserve);
    Task<IEnumerable<Reserve>> GetReservesByStoreIdAsync(int storeId);
    Task<Reserve?> GetReserveAsync(int storeId, int productId);
    Task UpdateReserveAsync(Reserve Reserve);
    Task DeleteReserveAsync(int storeId, int productId);
}
