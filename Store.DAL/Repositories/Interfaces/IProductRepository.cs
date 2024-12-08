using Store.DAL.DTO;

namespace Store.DAL.Repositories.Interfaces;

public interface IProductRepository
{
    Task AddProductAsync(ProductDTO product);
    Task<ProductDTO?> GetProductByNameAsync(string name);
    Task<ProductDTO?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
}
