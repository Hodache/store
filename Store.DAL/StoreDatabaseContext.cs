using Microsoft.EntityFrameworkCore;
using Store.DAL.Entities;

namespace Store.DAL;

public class StoreDatabaseContext : DbContext
{
    public DbSet<ShopEntity> Shops { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<ReserveEntity> Reserves { get; set; } = null!;

    public StoreDatabaseContext(DbContextOptions<StoreDatabaseContext> options)
    : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<ShopEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.HasMany(e => e.Reserves).WithOne(e => e.Shop).HasForeignKey(e => e.ShopId);
        });

        modelBuilder.Entity<ReserveEntity>(entity =>
        {
            entity.HasKey(e => new { e.ShopId, e.ProductId });
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).IsRequired();
            entity.HasOne(e => e.Shop).WithMany(e => e.Reserves).HasForeignKey(e => e.ShopId);
            entity.HasOne(e => e.Product).WithMany(e => e.Reserves).HasForeignKey(e => e.ProductId);
        });
    }
}
