using DatabaseDAL.PersistenceModel;
using Domain.DomainModel;

namespace DatabaseDAL
{
    internal static class ModelMapper
    {
        internal static Store ToDomain(StoreEntity storeEntity)
        {
            return new Store(storeEntity.Id, storeEntity.Name, storeEntity.Address);
        }

        internal static StoreEntity ToEntity(Store store)
        {
            return new StoreEntity
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address
            };
        }

        internal static Product ToDomain(ProductEntity productEntity)
        {
            return new Product(productEntity.Id, productEntity.Name);
        }

        internal static ProductEntity ToEntity(Product product)
        {
            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name
            };
        }

        internal static Reserve ToDomain(ReserveEntity reserveEntity)
        {
            return new Reserve(
                store: ToDomain(reserveEntity.Store),
                product: ToDomain(reserveEntity.Product),
                quantity: reserveEntity.Quantity,
                price: reserveEntity.Price
            );
        }

        internal static ReserveEntity ToEntity(Reserve reserve)
        {
            return new ReserveEntity
            {
                StoreId = reserve.Store.Id,
                ProductId = reserve.Product.Id,
                Quantity = reserve.Quantity,
                Price = reserve.Price
            };
        }
    }
}
