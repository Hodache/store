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
    public async Task CreateStore(StoreDto storeDto)
    {
        var store = new Store(storeDto.Name, storeDto.Address);
        await _storeRepository.AddStoreAsync(store);
    }

    // 2. Создать товар
    public async Task CreateProduct(ProductDto productDto)
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
        var store = await _storeRepository.GetStoreByIdAsync(storeId);
        if (store == null)
        {
            throw new ArgumentException("Store not found");
        }

        reserveEntries.ForEach(async entry =>
        {
            var product = _productRepository.GetProductByIdAsync(entry.ProductId).Result;
            if (product != null)
            {
                var reserve = _reserveRepository.GetReserveAsync(store.Id, product.Id).Result;
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
        });
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
            ProductId = cheapestReserve.Product.Id,
            ProductName = cheapestReserve.Product.Name,
            Price = cheapestReserve.Price
        };
    }

    // 5. Понять, какие товары можно купить в магазине на некоторую сумму
    // (например, на 100 рублей можно купить три мороженки или две шоколадки)
    public async Task<List<PurchaseOptionDto>> FindPurchasableProducts(int storeId, decimal budget)
    {
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
            if (quantity != 0 && quantity < reserve.Quantity)
            {
                purchaseOptions.Add(new PurchaseOptionDto
                {
                    ProductId = reserve.Product.Id,
                    ProductName = reserve.Product.Name,
                    Quantity = quantity
                });
            }
        }

        return purchaseOptions;
    }

    // 6. Купить партию товаров в магазине (параметры - сколько каких товаров купить,
    //метод возвращает общую стоимость покупки либо её невозможность, если товара не хватает)
    public async Task<PurchaseResultDto> PurchaseProducts(int storeId, List<PurchaseDto> purchaseEntries)
    {
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
    public async Task<CheapestStoreDto> FindCheapestStoreForBundle(List<ReserveEntryDto> productBundle)
    {
        var stores = await _storeRepository.GetAllStoresAsync();
        Dictionary<Store, decimal> storesTotal = [];

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
                    storesTotal[store] = decimal.MaxValue;
                }
                else
                {
                    if (!storesTotal.ContainsKey(store))
                    {
                        storesTotal[store] = 0;
                    }

                    storesTotal[store] += reserve.Price * entry.Quantity;
                }
            }
        }

        var cheapestStore = storesTotal.OrderBy(s => s.Value).First();
        return new CheapestStoreDto
        {
            StoreId = cheapestStore.Key.Id,
            StoreName = cheapestStore.Key.Name,
            Price = cheapestStore.Value
        };
    }
}
