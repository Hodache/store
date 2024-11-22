using Domain.DomainModel;

namespace Domain.AbstractRepositories;

public interface IReserveRepository
{
    Task AddReserveAsync(Reserve reserve);
    Task<IEnumerable<Reserve>> GetReservesByStoreIdAsync(int storeId);
    Task UpdateReserveAsync(Reserve reserve);
    Task DeleteReserveAsync(int storeId, int productId);
}
