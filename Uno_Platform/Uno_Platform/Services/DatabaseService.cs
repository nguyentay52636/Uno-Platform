using Uno_Platform.Database;
using Uno_Platform.Models;

namespace Uno_Platform.Services;

/// <summary>
/// Service layer cho database operations. Tự động chọn InMemory (WASM) hoặc SQLite (Android/Windows).
/// </summary>
public class DatabaseService
{
#if __WASM__
    private readonly InMemoryDbContext _dbContext;

    /// <summary>
    /// Constructor cho WebAssembly - sử dụng InMemoryDbContext (data trong RAM)
    /// </summary>
    public DatabaseService()
    {
        _dbContext = new InMemoryDbContext();
    }

    /// <summary>
    /// [WASM] Lấy tất cả sản phẩm từ RAM
    /// </summary>
    public List<Product> GetAllProducts()
    {
        return _dbContext.GetAllProducts();
    }

    /// <summary>
    /// [WASM] Tìm sản phẩm theo ID từ RAM
    /// </summary>
    public Product? GetProductById(int id)
    {
        return _dbContext.GetProductById(id);
    }

    /// <summary>
    /// [WASM] Thêm sản phẩm vào RAM. Returns true nếu thành công.
    /// </summary>
    public bool AddProduct(Product product)
    {
        try
        {
            _dbContext.AddProduct(product);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [WASM] Cập nhật sản phẩm trong RAM
    /// </summary>
    public bool UpdateProduct(Product product)
    {
        try
        {
            _dbContext.UpdateProduct(product);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [WASM] Xóa sản phẩm khỏi RAM
    /// </summary>
    public bool DeleteProduct(int id)
    {
        try
        {
            _dbContext.DeleteProduct(id);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [WASM] Tạo dữ liệu mẫu nếu database trống (chỉ chạy lần đầu)
    /// </summary>
    public void SeedSampleData()
    {
        try
        {
            var existingProducts = GetAllProducts();
            if (existingProducts.Count == 0)
            {
                var sampleProducts = new List<Product>
                {
                    new Product { Name = "Laptop", Price = 999.99m, Description = "High-performance laptop for work and gaming", Image = "💻" },
                    new Product { Name = "Smartphone", Price = 699.99m, Description = "Latest smartphone with advanced features", Image = "📱" },
                    new Product { Name = "Headphones", Price = 199.99m, Description = "Wireless noise-cancelling headphones", Image = "🎧" },
                    new Product { Name = "Tablet", Price = 499.99m, Description = "10-inch tablet perfect for reading and browsing", Image = "📱" },
                    new Product { Name = "Smartwatch", Price = 299.99m, Description = "Fitness tracking smartwatch", Image = "⌚" }
                };

                foreach (var product in sampleProducts)
                {
                    AddProduct(product);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
#else
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Constructor cho Android/Windows - sử dụng AppDbContext (SQLite persistent storage)
    /// </summary>
    public DatabaseService()
    {
        _dbContext = new AppDbContext();
    }

    /// <summary>
    /// [Android/Windows] Lấy tất cả sản phẩm từ SQLite database. Hiển thị toast nếu có lỗi.
    /// </summary>
    public List<Product> GetAllProducts()
    {
        try
        {
            return _dbContext.Connection.Table<Product>().ToList();
        }
        catch (SQLite.SQLiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SQLite error getting all products: {ex.Message}");
            ToastService.Instance.ShowError("Database error. Please try again.");
            return new List<Product>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting all products: {ex.Message}");
            ToastService.Instance.ShowError("Error loading products.");
            return new List<Product>();
        }
    }

    /// <summary>
    /// [Android/Windows] Tìm sản phẩm theo ID từ SQLite. Returns null nếu không tìm thấy hoặc có lỗi.
    /// </summary>
    public Product? GetProductById(int id)
    {
        try
        {
            return _dbContext.Connection.Table<Product>().FirstOrDefault(p => p.Id == id);
        }
        catch (SQLite.SQLiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SQLite error getting product by ID: {ex.Message}");
            ToastService.Instance.ShowError("Database error.");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting product by ID: {ex.Message}");
            ToastService.Instance.ShowError("Error loading product.");
            return null;
        }
    }

    /// <summary>
    /// [Android/Windows] Thêm sản phẩm vào SQLite. Returns true nếu insert thành công (affected rows > 0).
    /// </summary>
    public bool AddProduct(Product product)
    {
        try
        {
            int result = _dbContext.Connection.Insert(product);
            return result > 0;
        }
        catch (SQLite.SQLiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SQLite error adding product: {ex.Message}");
            ToastService.Instance.ShowError("Database error. Product not added.");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding product: {ex.Message}");
            ToastService.Instance.ShowError("Error adding product.");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Cập nhật sản phẩm trong SQLite. Returns true nếu update thành công.
    /// </summary>
    public bool UpdateProduct(Product product)
    {
        try
        {
            int result = _dbContext.Connection.Update(product);
            return result > 0;
        }
        catch (SQLite.SQLiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SQLite error updating product: {ex.Message}");
            ToastService.Instance.ShowError("Database error. Product not updated.");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating product: {ex.Message}");
            ToastService.Instance.ShowError("Error updating product.");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Xóa sản phẩm khỏi SQLite theo ID. Returns true nếu delete thành công.
    /// </summary>
    public bool DeleteProduct(int id)
    {
        try
        {
            int result = _dbContext.Connection.Delete<Product>(id);
            return result > 0;
        }
        catch (SQLite.SQLiteException ex)
        {
            System.Diagnostics.Debug.WriteLine($"SQLite error deleting product: {ex.Message}");
            ToastService.Instance.ShowError("Database error. Product not deleted.");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting product: {ex.Message}");
            ToastService.Instance.ShowError("Error deleting product.");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Tạo dữ liệu mẫu nếu SQLite database trống (chỉ chạy lần đầu)
    /// </summary>
    public void SeedSampleData()
    {
        try
        {
            var existingProducts = GetAllProducts();
            if (existingProducts.Count == 0)
            {
                var sampleProducts = new List<Product>
                {
                    new Product { Name = "Laptop", Price = 999.99m, Description = "High-performance laptop for work and gaming", Image = "💻" },
                    new Product { Name = "Smartphone", Price = 699.99m, Description = "Latest smartphone with advanced features", Image = "📱" },
                    new Product { Name = "Headphones", Price = 199.99m, Description = "Wireless noise-cancelling headphones", Image = "🎧" },
                    new Product { Name = "Tablet", Price = 499.99m, Description = "10-inch tablet perfect for reading and browsing", Image = "📱" },
                    new Product { Name = "Smartwatch", Price = 299.99m, Description = "Fitness tracking smartwatch", Image = "⌚" }
                };

                foreach (var product in sampleProducts)
                {
                    AddProduct(product);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
#endif
}

