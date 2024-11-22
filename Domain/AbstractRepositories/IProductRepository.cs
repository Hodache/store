using Domain.DomainModel;

namespace Domain.AbstractRepositories;

public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product?> GetProductByNameAsync(string name);
    Task<IEnumerable<Product>> GetAllProductsAsync();
}
