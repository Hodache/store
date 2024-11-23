using DTOs;

namespace AbstractServices;

public interface IStoreService
{
    Task CreateStore(CreateStoreDto storeDto);
    Task CreateProduct(CreateProductDto productDto);
    Task AddReserveToStore(int storeId, List<ReserveEntryDto> reserveEntries);
    Task<CheapestStoreDto> FindCheapestStoreForProduct(int productId);
    Task<List<PurchaseOptionDto>> FindPurchasableProducts(int storeId, decimal budget);
    Task<PurchaseResultDto> PurchaseProducts(int storeId, List<PurchaseDto> purchaseEntries);
    Task<CheapestStoreDto> FindCheapestStoreForBundle(List<PurchaseDto> productBundle);

    Task<List<StoreDto>> GetAllStores();
    Task<List<ProductDto>> GetAllProducts();
    Task<List<ReserveDto>> GetAllReserves();
    Task<List<ReserveDto>> GetReservesByStoreId(int storeId);
    Task<List<ReserveDto>> GetReservesByProductId(int productId);
}
