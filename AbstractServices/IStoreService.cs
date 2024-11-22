using DTOs;

namespace AbstractServices;

public interface IStoreService
{
    Task CreateStore(StoreDto storeDto);
    Task CreateProduct(ProductDto productDto);
    Task AddReserveToStore(int storeId, List<ReserveEntryDto> reserveEntries);
    Task<CheapestStoreDto> FindCheapestStoreForProduct(int productId);
    Task<List<PurchaseOptionDto>> FindPurchasableProducts(int storeId, decimal budget);
    Task<PurchaseResultDto> PurchaseProducts(int storeId, List<PurchaseDto> purchaseEntries);
    Task<CheapestStoreDto> FindCheapestStoreForBundle(List<ReserveEntryDto> productBundle);
}
