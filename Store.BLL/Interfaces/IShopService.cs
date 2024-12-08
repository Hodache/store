using Store.BLL.DTO;

namespace Store.BLL.Interfaces;

public interface IStoreService
{
    Task CreateShop(CreateShopDto shopDto);
    Task CreateProduct(CreateProductDto productDto);
    Task AddReserveToShop(int shopId, List<ReserveEntryDto> reserveEntries);
    Task<CheapestShopDto> FindCheapestShopForProduct(int productId);
    Task<List<PurchaseOptionDto>> FindPurchasableProducts(int shopId, decimal budget);
    Task<PurchaseResultDto> PurchaseProducts(int shopId, List<PurchaseDto> purchaseEntries);
    Task<CheapestShopDto> FindCheapestShopForBundle(List<PurchaseDto> productBundle);

    Task<List<ShopDto>> GetAllShops();
    Task<List<ProductDto>> GetAllProducts();
    Task<List<ReserveDto>> GetAllReserves();
    Task<List<ReserveDto>> GetReservesByShopId(int shopId);
    Task<List<ReserveDto>> GetReservesByProductId(int productId);
}
