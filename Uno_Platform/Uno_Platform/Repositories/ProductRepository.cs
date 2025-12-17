using Uno_Platform.Database;
using Uno_Platform.Models;
using Uno_Platform.Services;

namespace Uno_Platform.Repositories;

/// <summary>
/// Repository cho Product CRUD operations. Ưu tiên gọi API, fallback về InMemory nếu API fail.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly InMemoryDbContext _dbContext;
    private readonly ApiService _apiService;

    public ProductRepository()
    {
        _dbContext = new InMemoryDbContext();
        _apiService = ServiceLocator.ApiService;
        System.Diagnostics.Debug.WriteLine("=== ProductRepository: Initialized with API Service ===");
    }

    /// <summary>
    /// Lấy tất cả sản phẩm. Ưu tiên từ API, nếu fail thì dùng InMemory fallback.
    /// </summary>
    public async Task<List<Product>> GetAllProductsAsync()
    {
        // Ưu tiên gọi API, nếu thất bại thì dùng InMemory
        try
        {
            var products = await _apiService.GetProductsAsync();
            if (products != null && products.Any())
            {
                System.Diagnostics.Debug.WriteLine($"=== ProductRepository: Got {products.Count} products from API ===");
                return products;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"=== ProductRepository: API call failed, using fallback: {ex.Message} ===");
        }

        // Fallback: dùng InMemory data
        var fallbackProducts = _dbContext.GetAllProducts();
        System.Diagnostics.Debug.WriteLine($"=== ProductRepository: Using fallback, returned {fallbackProducts.Count} products. First item: {fallbackProducts.FirstOrDefault()?.Name} ===");
        return fallbackProducts;
    }

    /// <summary>
    /// Tìm sản phẩm theo ID từ InMemory database
    /// </summary>
    public Task<Product?> GetProductByIdAsync(int id)
    {
        var p = _dbContext.GetProductById(id);
        System.Diagnostics.Debug.WriteLine($"=== ProductRepository: GetProductById({id}) returned {p?.Name} Price={p?.Price} ===");
        return Task.FromResult(p);
    }

    /// <summary>
    /// Tìm kiếm sản phẩm theo keyword (tìm trong Name, Category, Description). Case-insensitive.
    /// </summary>
    public Task<List<Product>> SearchProductsAsync(string keyword)
    {
        var all = _dbContext.GetAllProducts();
        if (string.IsNullOrWhiteSpace(keyword))
            return Task.FromResult(all);

        var lowerKeyword = keyword.ToLowerInvariant();
        var results = all.Where(p =>
            p.Name.ToLowerInvariant().Contains(lowerKeyword) ||
            p.Category.ToLowerInvariant().Contains(lowerKeyword) ||
            p.Description.ToLowerInvariant().Contains(lowerKeyword)
        ).ToList();
        return Task.FromResult(results);
    }

    /// <summary>
    /// Lấy tất cả sản phẩm thuộc category. Case-insensitive comparison.
    /// </summary>
    public Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        var all = _dbContext.GetAllProducts();
        var results = all.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult(results);
    }

    /// <summary>
    /// Lấy sản phẩm trong khoảng giá từ minPrice đến maxPrice (inclusive)
    /// </summary>
    public Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var all = _dbContext.GetAllProducts();
        var results = all.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        return Task.FromResult(results);
    }

    /// <summary>
    /// Thêm sản phẩm mới vào database. Giữ nguyên ảnh từ product nếu đã có, nếu không thì dùng ảnh mặc định.
    /// </summary>
    public Task<bool> AddProductAsync(Product product)
    {
        // Chỉ set ảnh mặc định nếu chưa có ảnh
        if (string.IsNullOrWhiteSpace(product.Image))
        {
            product.Image = "Assets/img/caby.png";
        }
        _dbContext.AddProduct(product);
        System.Diagnostics.Debug.WriteLine($"=== ProductRepository: Added product {product.Name} with image {product.Image} ===");
        return Task.FromResult(true);
    }

    /// <summary>
    /// Cập nhật sản phẩm. Giữ nguyên ảnh từ product nếu đã có, nếu không thì dùng ảnh mặc định.
    /// </summary>
    public Task<bool> UpdateProductAsync(Product product)
    {
        // Chỉ set ảnh mặc định nếu chưa có ảnh
        if (string.IsNullOrWhiteSpace(product.Image))
        {
            product.Image = "Assets/img/caby.png";
        }
        _dbContext.UpdateProduct(product);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Xóa sản phẩm theo ID
    /// </summary>
    public Task<bool> DeleteProductAsync(int id)
    {
        _dbContext.DeleteProduct(id);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Lấy danh sách tất cả categories (unique, sorted alphabetically)
    /// </summary>
    public Task<List<string>> GetAllCategoriesAsync()
    {
        var all = _dbContext.GetAllProducts();
        var categories = all.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        return Task.FromResult(categories);
    }
}
