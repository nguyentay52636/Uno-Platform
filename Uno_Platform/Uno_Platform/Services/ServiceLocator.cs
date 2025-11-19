using Uno_Platform.Repositories;
#if !__WASM__
using Uno_Platform.Database;
#endif

namespace Uno_Platform.Services;

public static class ServiceLocator
{
    private static IProductRepository? _productRepository;
    private static ICartRepository? _cartRepository;
    private static ProductService? _productService;
    private static CartService? _cartService;
    private static NavigationService? _navigationService;
    private static DataSeedService? _dataSeedService;

#if !__WASM__
    private static AppDbContext? _dbContext;
    
    public static AppDbContext DbContext
    {
        get
        {
            if (_dbContext == null)
            {
                System.Diagnostics.Debug.WriteLine("=== ServiceLocator: Creating new AppDbContext ===");
                _dbContext = new AppDbContext();
            }
            return _dbContext;
        }
    }
#endif

    public static IProductRepository ProductRepository
    {
        get
        {
            _productRepository ??= new ProductRepository();
            return _productRepository;
        }
    }

    public static ICartRepository CartRepository
    {
        get
        {
            _cartRepository ??= new CartRepository();
            return _cartRepository;
        }
    }

    public static ProductService ProductService
    {
        get
        {
            _productService ??= new ProductService(ProductRepository);
            return _productService;
        }
    }

    public static CartService CartService
    {
        get
        {
            _cartService ??= new CartService(CartRepository, ProductRepository);
            return _cartService;
        }
    }

    public static NavigationService NavigationService
    {
        get
        {
            _navigationService ??= new NavigationService();
            return _navigationService;
        }
    }

    public static DataSeedService DataSeedService
    {
        get
        {
            _dataSeedService ??= new DataSeedService(ProductRepository);
            return _dataSeedService;
        }
    }

    public static ToastService ToastService => ToastService.Instance;
}
