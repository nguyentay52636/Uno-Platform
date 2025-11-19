using Uno_Platform.Database;
using Uno_Platform.Models;

namespace Uno_Platform.Repositories;

public class ProductRepository : IProductRepository
{
    // Use InMemoryDbContext for all platforms for simplicity and reliability
    private readonly InMemoryDbContext _dbContext;

    public ProductRepository()
    {
        _dbContext = new InMemoryDbContext();
        System.Diagnostics.Debug.WriteLine("=== ProductRepository: Using InMemoryDbContext ===");
    }

    public Task<List<Product>> GetAllProductsAsync()
    {
        var products = _dbContext.GetAllProducts();
        System.Diagnostics.Debug.WriteLine($"=== ProductRepository: GetAllProductsAsync returned {products.Count} products ===");
        return Task.FromResult(products);
    }

    public Task<Product?> GetProductByIdAsync(int id)
    {
        return Task.FromResult(_dbContext.GetProductById(id));
    }

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

    public Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        var all = _dbContext.GetAllProducts();
        var results = all.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult(results);
    }

    public Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var all = _dbContext.GetAllProducts();
        var results = all.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        return Task.FromResult(results);
    }

    public Task<bool> AddProductAsync(Product product)
    {
        product.Image = "Assets/img/caby.png"; // Always use default image
        _dbContext.AddProduct(product);
        System.Diagnostics.Debug.WriteLine($"=== ProductRepository: Added product {product.Name} ===");
        return Task.FromResult(true);
    }

    public Task<bool> UpdateProductAsync(Product product)
    {
        product.Image = "Assets/img/caby.png"; // Always use default image
        _dbContext.UpdateProduct(product);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        _dbContext.DeleteProduct(id);
        return Task.FromResult(true);
    }

    public Task<List<string>> GetAllCategoriesAsync()
    {
        var all = _dbContext.GetAllProducts();
        var categories = all.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
        return Task.FromResult(categories);
    }
}
