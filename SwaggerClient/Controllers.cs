using AbstractServices;
using DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SwaggerClient;

[ApiController]
[Route("api/[controller]")]
public class Controllers : ControllerBase
{
    private readonly IStoreService _storeService;

    public Controllers(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost("create-store")]
    public async Task<IActionResult> CreateStore([FromBody] CreateStoreDto storeDto)
    {
        await _storeService.CreateStore(storeDto);
        return Ok();
    }

    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        await _storeService.CreateProduct(productDto);
        return Ok();
    }

    [HttpPost("add-reserve")]
    public async Task<IActionResult> AddReserveToStore(int storeId, [FromBody] List<ReserveEntryDto> reserveEntries)
    {
        await _storeService.AddReserveToStore(storeId, reserveEntries);
        return Ok();
    }

    [HttpGet("cheapest-store-for-product")]
    public async Task<IActionResult> FindCheapestStoreForProduct(int productId)
    {
        CheapestStoreDto cheapestStore = await _storeService.FindCheapestStoreForProduct(productId);
        return Ok(cheapestStore);
    }

    [HttpGet("purchasable-products")]
    public async Task<IActionResult> FindPurchasableProducts(int storeId, decimal budget)
    {
        var purchasableProducts = await _storeService.FindPurchasableProducts(storeId, budget);
        return Ok(purchasableProducts);
    }

    [HttpPost("purchase-products")]
    public async Task<IActionResult> PurchaseProducts(int storeId, [FromBody] List<PurchaseDto> purchaseEntries)
    {
        var purchaseResult = await _storeService.PurchaseProducts(storeId, purchaseEntries);
        return Ok(purchaseResult);
    }

    [HttpPost("cheapest-store-for-bundle")]
    public async Task<IActionResult> FindCheapestStoreForBundle([FromBody] List<PurchaseDto> productBundle)
    {
        var cheapestStore = await _storeService.FindCheapestStoreForBundle(productBundle);
        return Ok(cheapestStore);
    }

    [HttpGet("stores")]
    public async Task<IActionResult> GetAllStores()
    {
        var stores = await _storeService.GetAllStores();
        return Ok(stores);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _storeService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("reserves")]
    public async Task<IActionResult> GetAllReserves()
    {
        var reserves = await _storeService.GetAllReserves();
        return Ok(reserves);
    }

    [HttpGet("reserves-by-store")]
    public async Task<IActionResult> GetReservesByStoreId(int storeId)
    {
        var reserves = await _storeService.GetReservesByStoreId(storeId);
        return Ok(reserves);
    }

    [HttpGet("reserves-by-product")]
    public async Task<IActionResult> GetReservesByProductId(int productId)
    {
        var reserves = await _storeService.GetReservesByProductId(productId);
        return Ok(reserves);
    }
}
