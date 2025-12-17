using Uno_Platform.Repositories;
#if !__WASM__
using Uno_Platform.Database;
#endif

namespace Uno_Platform.Services;

/// <summary>
/// Service Locator pattern - Singleton container cho tất cả services. Tránh dependency injection complexity.
/// </summary>
public static class ServiceLocator
{
    private static IProductRepository? _productRepository;
    private static ICartRepository? _cartRepository;
    private static ProductService? _productService;
    private static CartService? _cartService;
    private static NavigationService? _navigationService;
    private static DataSeedService? _dataSeedService;
    private static ApiService? _apiService;

#if !__WASM__
    private static AppDbContext? _dbContext;
    
    /// <summary>
    /// [Android/Windows Only] Singleton SQLite database context
    /// </summary>
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

    /// <summary>
    /// Singleton ProductRepository instance
    /// </summary>
    public static IProductRepository ProductRepository
    {
        get
        {
            _productRepository ??= new ProductRepository();
            return _productRepository;
        }
    }

    /// <summary>
    /// Singleton CartRepository instance
    /// </summary>
    public static ICartRepository CartRepository
    {
        get
        {
            _cartRepository ??= new CartRepository();
            return _cartRepository;
        }
    }

    /// <summary>
    /// Singleton ProductService instance (với ProductRepository dependency)
    /// </summary>
    public static ProductService ProductService
    {
        get
        {
            _productService ??= new ProductService(ProductRepository);
            return _productService;
        }
    }

    /// <summary>
    /// Singleton CartService instance (với CartRepository và ProductRepository dependencies)
    /// </summary>
    public static ICartService CartService
    {
        get
        {
            _cartService ??= new CartService(CartRepository, ProductRepository);
            return _cartService;
        }
    }

    /// <summary>
    /// Singleton NavigationService instance - quản lý navigation giữa các pages
    /// </summary>
    public static NavigationService NavigationService
    {
        get
        {
            _navigationService ??= new NavigationService();
            return _navigationService;
        }
    }

    /// <summary>
    /// Singleton DataSeedService instance - tạo dữ liệu mẫu khi app khởi động
    /// </summary>
    public static DataSeedService DataSeedService
    {
        get
        {
            _dataSeedService ??= new DataSeedService(ProductRepository);
            return _dataSeedService;
        }
    }

    /// <summary>
    /// Singleton ToastService instance - hiển thị toast notifications
    /// </summary>
    public static ToastService ToastService => ToastService.Instance;

    /// <summary>
    /// Singleton ApiService instance - gọi REST API backend
    /// </summary>
    public static ApiService ApiService
    {
        get
        {
            _apiService ??= new ApiService();
            return _apiService;
        }
    }
}
