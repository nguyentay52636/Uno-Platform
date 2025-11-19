using Uno_Platform.Database;
using Uno_Platform.Models;

namespace Uno_Platform.Repositories;

public class CartRepository : ICartRepository
{
    // Use InMemoryDbContext for all platforms
    private readonly InMemoryDbContext _dbContext;

    public CartRepository()
    {
        _dbContext = new InMemoryDbContext();
        System.Diagnostics.Debug.WriteLine("=== CartRepository: Using InMemoryDbContext ===");
    }

    public Task<List<CartItem>> GetAllCartItemsAsync()
    {
        return Task.FromResult(_dbContext.GetAllCartItems());
    }

    public Task<CartItem?> GetCartItemByProductIdAsync(int productId)
    {
        return Task.FromResult(_dbContext.GetCartItemByProductId(productId));
    }

    public Task<bool> AddCartItemAsync(CartItem item)
    {
        item.ProductImage = "Assets/img/caby.png"; // Always use default image
        _dbContext.AddCartItem(item);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateCartItemAsync(CartItem item)
    {
        item.ProductImage = "Assets/img/caby.png"; // Always use default image
        _dbContext.UpdateCartItem(item);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteCartItemAsync(int id)
    {
        _dbContext.DeleteCartItem(id);
        return Task.FromResult(true);
    }

    public Task<bool> ClearCartAsync()
    {
        _dbContext.ClearCart();
        return Task.FromResult(true);
    }

    public Task<int> GetCartItemCountAsync()
    {
        var items = _dbContext.GetAllCartItems();
        var count = items.Sum(item => item.Quantity);
        return Task.FromResult(count);
    }
}
