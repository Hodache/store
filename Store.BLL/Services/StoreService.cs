using System.Data;
using Store.BLL.Interfaces;
using Store.BLL.DTO;
using Store.DAL.Repositories.Interfaces;
using Store.DAL.DTO;

namespace Store.BLL.Services;

public class StoreService : IStoreService
{
    private readonly IShopRepository _shopRepository;
    private readonly IProductRepository _productRepository;
    private readonly IReserveRepository _reserveRepository;

    public StoreService(IShopRepository shopRepository, IProductRepository productRepository, IReserveRepository reserveRepository)
    {
        _shopRepository = shopRepository;
        _productRepository = productRepository;
        _reserveRepository = reserveRepository;
    }

    // 1. Создать магазин
    public async Task CreateShop(CreateShopDto shopDto)
    {
        var shop = new ShopDTO(shopDto.Name, shopDto.Address);
        await _shopRepository.AddShopAsync(shop);
    }

    // 2. Создать товар
    public async Task CreateProduct(CreateProductDto productDto)
    {
        ProductDTO? product = await _productRepository.GetProductByNameAsync(productDto.Name);
        if (product != null)
        {
            throw new ArgumentException("Product already exists");
        }

        var newProduct = new ProductDTO(productDto.Name);
        await _productRepository.AddProductAsync(newProduct);
    }

    // 3. Завезти партию товаров в магазин (набор товар-количество с возможностью установить/изменить цену)
    public async Task AddReserveToShop(int shopId, List<ReserveEntryDto> reserveEntries)
    {
        foreach (var entry in reserveEntries)
        {
            if (entry.Quantity < 0)
            {
                throw new ArgumentException("Quantity must be non negative");
            }
            else if (entry.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than 0");
            }
        }

        var shop = await _shopRepository.GetShopByIdAsync(shopId);
        if (shop == null)
        {
            throw new ArgumentException("Shop not found");
        }

        foreach (var entry in reserveEntries)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            var reserve = await _reserveRepository.GetReserveAsync(shop.Id, entry.ProductId);
            if (reserve == null)
            {
                var newReserve = new ReserveDTO(shopId, entry.ProductId, entry.Quantity, entry.Price);
                await _reserveRepository.AddReserveAsync(newReserve);
            }
            else
            {
                reserve.Quantity += entry.Quantity;
                reserve.Price = entry.Price;
                await _reserveRepository.UpdateReserveAsync(reserve);
            }
        }
    }

    // 4. Найти магазин, в котором определенный товар самый дешевый
    public async Task<CheapestShopDto> FindCheapestShopForProduct(int productId)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            throw new ArgumentException("Product not found");
        }

        var reserves = await _reserveRepository.GetReservesByProductIdAsync(product.Id);
        if (!reserves.Any())
        {
            throw new ArgumentException("No reserves found for product");
        }

        var cheapestReserve = reserves.OrderBy(r => r.Price).First();
        var shop = await _shopRepository.GetShopByIdAsync(cheapestReserve.ShopId);
        return new CheapestShopDto
        {
            ShopId = cheapestReserve.ShopId,
            ShopName = shop.Name,
            Price = cheapestReserve.Price
        };
    }

    // 5. Понять, какие товары можно купить в магазине на некоторую сумму
    // (например, на 100 рублей можно купить три мороженки или две шоколадки)
    public async Task<List<PurchaseOptionDto>> FindPurchasableProducts(int shopId, decimal budget)
    {
        if (budget <= 0)
        {
            throw new ArgumentException("Budget must be greater than 0");
        }

        var shop = await _shopRepository.GetShopByIdAsync(shopId);
        if (shop == null)
        {
            throw new ArgumentException("Shop not found");
        }

        var reserves = await _reserveRepository.GetReservesByShopIdAsync(shop.Id);
        if (!reserves.Any())
        {
            throw new ArgumentException("No reserves found for shop");
        }

        var purchaseOptions = new List<PurchaseOptionDto>();
        foreach (var reserve in reserves)
        {
            var quantity = (int)(budget / reserve.Price);
            if (quantity > 0)
            {
                var product = await _productRepository.GetProductByIdAsync(reserve.ProductId);

                purchaseOptions.Add(new PurchaseOptionDto
                {
                    ProductId = reserve.ProductId,
                    ProductName = product.Name,
                    Quantity = Math.Min(quantity, reserve.Quantity)
                });
            }
        }

        return purchaseOptions;
    }

    // 6. Купить партию товаров в магазине (параметры - сколько каких товаров купить,
    //метод возвращает общую стоимость покупки либо её невозможность, если товара не хватает)
    public async Task<PurchaseResultDto> PurchaseProducts(int shopId, List<PurchaseDto> purchaseEntries)
    {
        foreach (var entry in purchaseEntries)
        {
            if (entry.Quantity <= 0) 
            { 
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }

        var shop = await _shopRepository.GetShopByIdAsync(shopId);
        if (shop == null)
        {
            throw new ArgumentException("Shop not found");
        }

        decimal totalCost = 0;
        List<ReserveDTO> reserves = [];
        PurchaseResultDto result = new PurchaseResultDto { CanBuy = true };
        foreach (var entry in purchaseEntries)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            var reserve = await _reserveRepository.GetReserveAsync(shop.Id, product.Id);
            if (reserve == null || reserve.Quantity < entry.Quantity)
            {
                result.CanBuy = false;
                break;
            }

            totalCost += reserve.Price * entry.Quantity;
            reserve.Quantity -= entry.Quantity;
            reserves.Add(reserve);
        }
        
        if (result.CanBuy)
        {
            result.TotalCost = totalCost;
            foreach (var reserve in reserves)
            {
                await _reserveRepository.UpdateReserveAsync(reserve);
            }
        }
        
        return result;
    }

    // 7.Найти, в каком магазине партия товаров (набор товар-количество) имеет наименьшую сумму (в целом).
    // Например, «в каком магазине дешевле всего купить 10 гвоздей и 20 шурупов». 
    public async Task<CheapestShopDto> FindCheapestShopForBundle(List<PurchaseDto> productBundle)
    {
        foreach (var entry in productBundle)
        {
            if (entry.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }

        var shops = await _shopRepository.GetAllShopsAsync();
        Dictionary<int, decimal> shopsTotal = [];

        foreach (var entry in productBundle)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            foreach (var shop in shops)
            {
                var reserve = await _reserveRepository.GetReserveAsync(shop.Id, product.Id);
                if (reserve == null || reserve.Quantity < entry.Quantity)
                {
                    shopsTotal[shop.Id] = decimal.MaxValue;
                }
                else
                {
                    if (!shopsTotal.ContainsKey(shop.Id))
                    {
                        shopsTotal[shop.Id] = 0;
                    }

                    if (shopsTotal[shop.Id] != decimal.MaxValue)
                    {
                        shopsTotal[shop.Id] = shopsTotal[shop.Id] + reserve.Price * entry.Quantity;
                    }
                }
            }
        }

        var minPrice = shopsTotal.OrderBy(s => s.Value).First();

        if (minPrice.Value == decimal.MaxValue)
        {
            return new CheapestShopDto
            {
                ShopId = -1,
                ShopName = "No matching shop",
                Price = 0
            };
        }

        var cheapestShop = shops.First(s => s.Id == minPrice.Key);

        return new CheapestShopDto
        {
            ShopId = cheapestShop.Id,
            ShopName = cheapestShop.Name,
            Price = minPrice.Value
        };
    }

    public async Task<List<ShopDto>> GetAllShops()
    {
        var shops = await _shopRepository.GetAllShopsAsync();
        return shops.Select(s => new ShopDto
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address
        }).ToList();
    }

    public async Task<List<ProductDto>> GetAllProducts()
    {
        var products = await _productRepository.GetAllProductsAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name
        }).ToList();
    }

    public async Task<List<ReserveDto>> GetAllReserves()
    {
        var reserves = await _reserveRepository.GetAllReservesAsync();

        var result = await AttachShopAndProductToReserves(reserves);

        return result;
    }

    public async Task<List<ReserveDto>> GetReservesByShopId(int shopId)
    {
        var reserves = await _reserveRepository.GetReservesByShopIdAsync(shopId);

        var result = await AttachShopAndProductToReserves(reserves);

        return result;
    }

    public async Task<List<ReserveDto>> GetReservesByProductId(int productId)
    {
        var reserves = await _reserveRepository.GetReservesByProductIdAsync(productId);

        var result = await AttachShopAndProductToReserves(reserves);

        return result;
    }

    private async Task<List<ReserveDto>> AttachShopAndProductToReserves(IEnumerable<ReserveDTO> reserves)
    {
        List<ReserveDto> result = [];
        foreach (var r in reserves)
        {
            var shop = await _shopRepository.GetShopByIdAsync(r.ShopId);
            var product = await _productRepository.GetProductByIdAsync(r.ProductId);

            result.Add(
                new ReserveDto
                {
                    ShopId = r.ShopId,
                    ShopName = shop.Name,
                    ProductId = r.ProductId,
                    ProductName = product.Name,
                    Quantity = r.Quantity,
                    Price = r.Price
                });
        }

        return result;
    }
}
