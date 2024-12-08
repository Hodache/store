using Store.BLL.Interfaces;
using Store.BLL.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Controllers : ControllerBase
{
    private readonly IStoreService _storeService;

    public Controllers(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost("create-shop")]
    public async Task<IActionResult> CreateShop([FromBody] CreateShopDto shopDto)
    {
        await _storeService.CreateShop(shopDto);
        return Ok();
    }

    [HttpPost("create-product")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        await _storeService.CreateProduct(productDto);
        return Ok();
    }

    [HttpPost("add-reserve")]
    public async Task<IActionResult> AddReserveToShop(int shopId, [FromBody] List<ReserveEntryDto> reserveEntries)
    {
        await _storeService.AddReserveToShop(shopId, reserveEntries);
        return Ok();
    }

    [HttpGet("cheapest-shop-for-product")]
    public async Task<IActionResult> FindCheapestShopForProduct(int productId)
    {
        CheapestShopDto cheapestShop = await _storeService.FindCheapestShopForProduct(productId);
        return Ok(cheapestShop);
    }

    [HttpGet("purchasable-products")]
    public async Task<IActionResult> FindPurchasableProducts(int shopId, decimal budget)
    {
        var purchasableProducts = await _storeService.FindPurchasableProducts(shopId, budget);
        return Ok(purchasableProducts);
    }

    [HttpPost("purchase-products")]
    public async Task<IActionResult> PurchaseProducts(int shopId, [FromBody] List<PurchaseDto> purchaseEntries)
    {
        var purchaseResult = await _storeService.PurchaseProducts(shopId, purchaseEntries);
        return Ok(purchaseResult);
    }

    [HttpPost("cheapest-shop-for-bundle")]
    public async Task<IActionResult> FindCheapestShopForBundle([FromBody] List<PurchaseDto> productBundle)
    {
        var cheapestShop = await _storeService.FindCheapestShopForBundle(productBundle);
        return Ok(cheapestShop);
    }

    [HttpGet("shops")]
    public async Task<IActionResult> GetAllShops()
    {
        var shops = await _storeService.GetAllShops();
        return Ok(shops);
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

    [HttpGet("reserves-by-shop")]
    public async Task<IActionResult> GetReservesByShopId(int shopId)
    {
        var reserves = await _storeService.GetReservesByShopId(shopId);
        return Ok(reserves);
    }

    [HttpGet("reserves-by-product")]
    public async Task<IActionResult> GetReservesByProductId(int productId)
    {
        var reserves = await _storeService.GetReservesByProductId(productId);
        return Ok(reserves);
    }
}
