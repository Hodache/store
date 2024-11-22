using Domain.AbstractRepositories;
using Domain.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace DatabaseDAL.Repositories;

internal class ProductRepository : IProductRepository
{
    private readonly StoreDatabaseContext _context;

    public ProductRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
        var productEntity = ModelMapper.ToEntity(product);

        _context.Products.Add(productEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var productEntities = await _context.Products.ToListAsync();
        return productEntities.Select(ModelMapper.ToDomain);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var productEntity = await _context.Products.FindAsync(id);
        return productEntity == null ? null : ModelMapper.ToDomain(productEntity);
    }
}
