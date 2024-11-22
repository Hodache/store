using Domain.DomainModel;

namespace Domain.AbstractRepositories;

public interface IStoreRepository
{
    Task AddStoreAsync(Store store);
    Task<Store?> GetStoreByIdAsync(int id);
    Task<IEnumerable<Store>> GetAllStoresAsync();
}
