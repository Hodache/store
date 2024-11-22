using Microsoft.EntityFrameworkCore;
using DatabaseDAL.PersistenceModel;

namespace DatabaseDAL;


internal class StoreDatabaseContext : DbContext
{
    public DbSet<StoreEntity> Stores { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<ReserveEntity> Reserves { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=StoreDatabase;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<StoreEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.HasMany(e => e.Reserves).WithOne(e => e.Store).HasForeignKey(e => e.StoreId);
        });

        modelBuilder.Entity<ReserveEntity>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.ProductId });
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).IsRequired();
            entity.HasOne(e => e.Store).WithMany(e => e.Reserves).HasForeignKey(e => e.StoreId);
            entity.HasOne(e => e.Product).WithMany(e => e.Reserves).HasForeignKey(e => e.ProductId);
        });
    }
}
