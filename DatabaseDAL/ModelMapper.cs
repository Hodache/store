using DatabaseDAL.PersistenceModel;
using Domain.DomainModel;

namespace DatabaseDAL
{
    internal static class ModelMapper
    {
        public static Store FromStoreEntity(StoreEntity storeEntity)
        {
            return new Store(storeEntity.Id, storeEntity.Name, storeEntity.Address);
        }

        public static StoreEntity ToStoreEntity(Store store)
        {
            return new StoreEntity
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address
            };
        }

        public static Product FromProductEntity(ProductEntity productEntity)
        {
            return new Product(productEntity.Id, productEntity.Name);
        }

        public static ProductEntity ToProductEntity(Product product)
        {
            return new ProductEntity
            {
                Id = product.Id,
                Name = product.Name
            };
        }

        public static Reserve FromReserveEntity(ReserveEntity reserveEntity)
        {
            return new Reserve(
                store: FromStoreEntity(reserveEntity.Store),
                product: FromProductEntity(reserveEntity.Product),
                quantity: reserveEntity.Quantity,
                price: reserveEntity.Price
            );
        }

        public static ReserveEntity ToReserveEntity(Reserve reserve)
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
