using Uno_Platform.Database;
using Uno_Platform.Models;
#if !__WASM__
using Microsoft.EntityFrameworkCore;
#endif

namespace Uno_Platform.Repositories;

/// <summary>
/// Repository cho Cart CRUD operations. Có 2 implementations: EF Core (Android/Windows) và InMemory (WASM).
/// </summary>
public class CartRepository : ICartRepository
{
#if !__WASM__
    // ========== ANDROID/WINDOWS VERSION (EF CORE) ==========
    private readonly EfAppDbContext _dbContext;
    private bool _isInitialized = false;
    private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

    /// <summary>
    /// [Android/Windows] Constructor - sử dụng EF Core với SQLite
    /// </summary>
    public CartRepository()
    {
        _dbContext = new EfAppDbContext();
        System.Diagnostics.Debug.WriteLine("=== CartRepository: Using EF Core SQLite ===");
    }

    /// <summary>
    /// [Android/Windows] Đảm bảo database đã được khởi tạo. Thread-safe với SemaphoreSlim.
    /// </summary>
    private async Task EnsureDatabaseInitializedAsync()
    {
        if (_isInitialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (!_isInitialized)
            {
                await _dbContext.Database.EnsureCreatedAsync();
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("=== Database initialized successfully ===");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
            throw;
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// [Android/Windows] Lấy tất cả cart items từ EF Core SQLite. Returns empty list nếu có lỗi.
    /// </summary>
    public async Task<List<CartItem>> GetAllCartItemsAsync()
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            return await _dbContext.CartItems.ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting cart items: {ex.Message}");
            return new List<CartItem>();
        }
    }

    /// <summary>
    /// [Android/Windows] Tìm cart item theo Product ID. Returns null nếu không tìm thấy hoặc có lỗi.
    /// </summary>
    public async Task<CartItem?> GetCartItemByProductIdAsync(int productId)
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            return await _dbContext.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting cart item by product ID: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// [Android/Windows] Thêm cart item vào EF Core SQLite. Tự động set ảnh mặc định.
    /// </summary>
    public async Task<bool> AddCartItemAsync(CartItem item)
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            item.ProductImage = "Assets/img/caby.png"; // Always use default image
            _dbContext.CartItems.Add(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding cart item: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Cập nhật cart item trong EF Core SQLite. Tự động set ảnh mặc định.
    /// </summary>
    public async Task<bool> UpdateCartItemAsync(CartItem item)
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            item.ProductImage = "Assets/img/caby.png"; // Always use default image
            _dbContext.CartItems.Update(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating cart item: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Xóa cart item khỏi EF Core SQLite theo ID. Returns false nếu không tìm thấy.
    /// </summary>
    public async Task<bool> DeleteCartItemAsync(int id)
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            var item = await _dbContext.CartItems.FindAsync(id);
            if (item != null)
            {
                _dbContext.CartItems.Remove(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting cart item: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Xóa toàn bộ cart từ EF Core SQLite (sau khi checkout thành công)
    /// </summary>
    public async Task<bool> ClearCartAsync()
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            var allItems = await _dbContext.CartItems.ToListAsync();
            _dbContext.CartItems.RemoveRange(allItems);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error clearing cart: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// [Android/Windows] Tính tổng số items trong cart (sum của quantities). Returns 0 nếu có lỗi.
    /// </summary>
    public async Task<int> GetCartItemCountAsync()
    {
        await EnsureDatabaseInitializedAsync();
        try
        {
            var items = await _dbContext.CartItems.ToListAsync();
            return items.Sum(item => item.Quantity);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting cart count: {ex.Message}");
            return 0;
        }
    }
#else
    // ========== WEBASSEMBLY VERSION (IN-MEMORY) ==========
    private readonly InMemoryDbContext _dbContext;

    /// <summary>
    /// [WASM] Constructor - sử dụng InMemoryDbContext (data trong RAM)
    /// </summary>
    public CartRepository()
    {
        _dbContext = new InMemoryDbContext();
        System.Diagnostics.Debug.WriteLine("=== CartRepository: Using InMemoryDbContext (WASM) ===");
    }

    /// <summary>
    /// [WASM] Lấy tất cả cart items từ RAM
    /// </summary>
    public Task<List<CartItem>> GetAllCartItemsAsync()
    {
        return Task.FromResult(_dbContext.GetAllCartItems());
    }

    /// <summary>
    /// [WASM] Tìm cart item theo Product ID từ RAM
    /// </summary>
    public Task<CartItem?> GetCartItemByProductIdAsync(int productId)
    {
        return Task.FromResult(_dbContext.GetCartItemByProductId(productId));
    }

    /// <summary>
    /// [WASM] Thêm cart item vào RAM. Tự động set ảnh mặc định.
    /// </summary>
    public Task<bool> AddCartItemAsync(CartItem item)
    {
        item.ProductImage = "Assets/img/caby.png";
        _dbContext.AddCartItem(item);
        return Task.FromResult(true);
    }

    /// <summary>
    /// [WASM] Cập nhật cart item trong RAM. Tự động set ảnh mặc định.
    /// </summary>
    public Task<bool> UpdateCartItemAsync(CartItem item)
    {
        item.ProductImage = "Assets/img/caby.png";
        _dbContext.UpdateCartItem(item);
        return Task.FromResult(true);
    }

    /// <summary>
    /// [WASM] Xóa cart item khỏi RAM theo ID
    /// </summary>
    public Task<bool> DeleteCartItemAsync(int id)
    {
        _dbContext.DeleteCartItem(id);
        return Task.FromResult(true);
    }

    /// <summary>
    /// [WASM] Xóa toàn bộ cart khỏi RAM (sau khi checkout thành công)
    /// </summary>
    public Task<bool> ClearCartAsync()
    {
        _dbContext.ClearCart();
        return Task.FromResult(true);
    }

    /// <summary>
    /// [WASM] Tính tổng số items trong cart (sum của quantities)
    /// </summary>
    public Task<int> GetCartItemCountAsync()
    {
        var items = _dbContext.GetAllCartItems();
        return Task.FromResult(items.Sum(item => item.Quantity));
    }
#endif
}
