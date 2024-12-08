using Microsoft.EntityFrameworkCore;
using Store.DAL.DTO;
using Store.DAL.Entities;
using Store.DAL.Repositories.Interfaces;

namespace Store.DAL.Repositories.Database;

public class ProductRepository : IProductRepository
{
    private readonly StoreDatabaseContext _context;

    public ProductRepository(StoreDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(ProductDTO product)
    {
        var productEntity = Mapper.ToEntity(product);

        _context.Products.Add(productEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        var productEntities = await _context.Products.ToListAsync();
        return productEntities.Select(Mapper.ToDomain);
    }

    public async Task<ProductDTO?> GetProductByIdAsync(int id)
    {
        var productEntity = await _context.Products.FindAsync(id);
        return productEntity == null ? null : Mapper.ToDomain(productEntity);
    }

    public async Task<ProductDTO?> GetProductByNameAsync(string name)
    {
        var productEntity = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        return productEntity == null ? null : Mapper.ToDomain(productEntity);
    }
}
