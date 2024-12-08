using Store.DAL.DTO;

namespace Store.DAL.Entities;

internal static class Mapper
{
    internal static ShopDTO ToDomain(ShopEntity shopEntity)
    {
        return new ShopDTO(shopEntity.Id, shopEntity.Name, shopEntity.Address);
    }

    internal static ShopEntity ToEntity(ShopDTO shop)
    {
        return new ShopEntity
        {
            Id = shop.Id,
            Name = shop.Name,
            Address = shop.Address
        };
    }

    internal static ProductDTO ToDomain(ProductEntity productEntity)
    {
        return new ProductDTO(productEntity.Id, productEntity.Name);
    }

    internal static ProductEntity ToEntity(ProductDTO product)
    {
        return new ProductEntity
        {
            Id = product.Id,
            Name = product.Name
        };
    }

    internal static ReserveDTO ToDomain(ReserveEntity reserveEntity)
    {
        return new ReserveDTO(
            shopId: reserveEntity.ShopId,
            productId: reserveEntity.ProductId,
            quantity: reserveEntity.Quantity,
            price: reserveEntity.Price
        );
    }

    internal static ReserveEntity ToEntity(ReserveDTO reserve)
    {
        return new ReserveEntity
        {
            ShopId = reserve.ShopId,
            ProductId = reserve.ProductId,
            Quantity = reserve.Quantity,
            Price = reserve.Price
        };
    }
}
