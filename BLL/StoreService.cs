using AbstractServices;
using Domain.AbstractRepositories;
using Domain.DomainModel;
using DTOs;
using System.Data;

namespace BLL;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IReserveRepository _reserveRepository;

    public StoreService(IStoreRepository storeRepository, IProductRepository productRepository, IReserveRepository reserveRepository)
    {
        _storeRepository = storeRepository;
        _productRepository = productRepository;
        _reserveRepository = reserveRepository;
    }

    // 1. Создать магазин
    public async Task CreateStore(CreateStoreDto storeDto)
    {
        var store = new Store(storeDto.Name, storeDto.Address);
        await _storeRepository.AddStoreAsync(store);
    }

    // 2. Создать товар
    public async Task CreateProduct(CreateProductDto productDto)
    {
        Product? product = await _productRepository.GetProductByNameAsync(productDto.Name);
        if (product != null)
        {
            throw new ArgumentException("Product already exists");
        }

        var newProduct = new Product(productDto.Name);
        await _productRepository.AddProductAsync(newProduct);
    }

    // 3. Завезти партию товаров в магазин (набор товар-количество с возможностью установить/изменить цену)
    public async Task AddReserveToStore(int storeId, List<ReserveEntryDto> reserveEntries)
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

        var store = await _storeRepository.GetStoreByIdAsync(storeId);
        if (store == null)
        {
            throw new ArgumentException("Store not found");
        }

        foreach (var entry in reserveEntries)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product != null)
            {
                var reserve = await _reserveRepository.GetReserveAsync(store.Id, product.Id);
                if (reserve == null)
                {
                    var newReserve = new Reserve(store, product, entry.Quantity, entry.Price);
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
    }

    // 4. Найти магазин, в котором определенный товар самый дешевый
    public async Task<CheapestStoreDto> FindCheapestStoreForProduct(int productId)
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
        return new CheapestStoreDto
        {
            StoreId = cheapestReserve.Store.Id,
            StoreName = cheapestReserve.Store.Name,
            Price = cheapestReserve.Price
        };
    }

    // 5. Понять, какие товары можно купить в магазине на некоторую сумму
    // (например, на 100 рублей можно купить три мороженки или две шоколадки)
    public async Task<List<PurchaseOptionDto>> FindPurchasableProducts(int storeId, decimal budget)
    {
        if (budget <= 0)
        {
            throw new ArgumentException("Budget must be greater than 0");
        }

        var store = await _storeRepository.GetStoreByIdAsync(storeId);
        if (store == null)
        {
            throw new ArgumentException("Store not found");
        }

        var reserves = await _reserveRepository.GetReservesByStoreIdAsync(store.Id);
        if (!reserves.Any())
        {
            throw new ArgumentException("No reserves found for store");
        }

        var purchaseOptions = new List<PurchaseOptionDto>();
        foreach (var reserve in reserves)
        {
            var quantity = (int)(budget / reserve.Price);
            if (quantity > 0)
            {
                purchaseOptions.Add(new PurchaseOptionDto
                {
                    ProductId = reserve.Product.Id,
                    ProductName = reserve.Product.Name,
                    Quantity = Math.Min(quantity, reserve.Quantity)
                });
            }
        }

        return purchaseOptions;
    }

    // 6. Купить партию товаров в магазине (параметры - сколько каких товаров купить,
    //метод возвращает общую стоимость покупки либо её невозможность, если товара не хватает)
    public async Task<PurchaseResultDto> PurchaseProducts(int storeId, List<PurchaseDto> purchaseEntries)
    {
        foreach (var entry in purchaseEntries)
        {
            if (entry.Quantity <= 0) 
            { 
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }

        var store = await _storeRepository.GetStoreByIdAsync(storeId);
        if (store == null)
        {
            throw new ArgumentException("Store not found");
        }

        decimal totalCost = 0;
        foreach (var entry in purchaseEntries)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            var reserve = await _reserveRepository.GetReserveAsync(store.Id, product.Id);
            if (reserve == null || reserve.Quantity < entry.Quantity)
            {
                return new PurchaseResultDto { CanBuy = false };
            }

            totalCost += reserve.Price * entry.Quantity;
            reserve.Quantity -= entry.Quantity;
            await _reserveRepository.UpdateReserveAsync(reserve);
        }

        return new PurchaseResultDto { CanBuy = true, TotalCost = totalCost };
    }

    // 7.Найти, в каком магазине партия товаров (набор товар-количество) имеет наименьшую сумму (в целом).
    // Например, «в каком магазине дешевле всего купить 10 гвоздей и 20 шурупов». 
    public async Task<CheapestStoreDto> FindCheapestStoreForBundle(List<PurchaseDto> productBundle)
    {
        foreach (var entry in productBundle)
        {
            if (entry.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0");
            }
        }

        var stores = await _storeRepository.GetAllStoresAsync();
        Dictionary<int, decimal> storesTotal = [];

        foreach (var entry in productBundle)
        {
            var product = await _productRepository.GetProductByIdAsync(entry.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            foreach (var store in stores)
            {
                var reserve = await _reserveRepository.GetReserveAsync(store.Id, product.Id);
                if (reserve == null || reserve.Quantity < entry.Quantity)
                {
                    storesTotal[store.Id] = decimal.MaxValue;
                }
                else
                {
                    if (!storesTotal.ContainsKey(store.Id))
                    {
                        storesTotal[store.Id] = 0;
                    }

                    if (storesTotal[store.Id] != decimal.MaxValue)
                    {
                        storesTotal[store.Id] = storesTotal[store.Id] + reserve.Price * entry.Quantity;
                    }
                }
            }
        }

        var minPrice = storesTotal.OrderBy(s => s.Value).First();
        var cheapestStore = stores.First(s => s.Id == minPrice.Key);

        return new CheapestStoreDto
        {
            StoreId = cheapestStore.Id,
            StoreName = cheapestStore.Name,
            Price = minPrice.Value
        };
    }

    public async Task<List<StoreDto>> GetAllStores()
    {
        var stores = await _storeRepository.GetAllStoresAsync();
        return stores.Select(s => new StoreDto
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
        return reserves.Select(r => new ReserveDto
        {
            StoreId = r.Store.Id,
            StoreName = r.Store.Name,
            ProductId = r.Product.Id,
            ProductName = r.Product.Name,
            Quantity = r.Quantity,
            Price = r.Price
        }).ToList();
    }

    public async Task<List<ReserveDto>> GetReservesByStoreId(int storeId)
    {
        var reserves = await _reserveRepository.GetReservesByStoreIdAsync(storeId);
        return reserves.Select(r => new ReserveDto
        {
            StoreId = r.Store.Id,
            StoreName = r.Store.Name,
            ProductId = r.Product.Id,
            ProductName = r.Product.Name,
            Quantity = r.Quantity,
            Price = r.Price
        }).ToList();
    }

    public async Task<List<ReserveDto>> GetReservesByProductId(int productId)
    {
        var reserves = await _reserveRepository.GetReservesByProductIdAsync(productId);
        return reserves.Select(r => new ReserveDto
        {
            StoreId = r.Store.Id,
            StoreName = r.Store.Name,
            ProductId = r.Product.Id,
            ProductName = r.Product.Name,
            Quantity = r.Quantity,
            Price = r.Price
        }).ToList();
    }
}
