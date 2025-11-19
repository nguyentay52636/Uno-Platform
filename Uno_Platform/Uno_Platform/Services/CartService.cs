using Uno_Platform.Models;
using Uno_Platform.Repositories;

namespace Uno_Platform.Services;

public class CartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public event EventHandler<int>? CartCountChanged;

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        return await _cartRepository.GetAllCartItemsAsync();
    }

    public async Task<int> GetCartItemCountAsync()
    {
        return await _cartRepository.GetCartItemCountAsync();
    }

    public async Task<bool> AddToCartAsync(int productId)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
            return false;

        var existingItem = await _cartRepository.GetCartItemByProductIdAsync(productId);
        
        bool result;
        if (existingItem != null)
        {
            existingItem.Quantity++;
            result = await _cartRepository.UpdateCartItemAsync(existingItem);
        }
        else
        {
            var newItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                ProductImage = "Assets/img/caby.png",
                ProductCategory = product.Category,
                Quantity = 1
            };
            result = await _cartRepository.AddCartItemAsync(newItem);
        }

        if (result)
        {
            var count = await GetCartItemCountAsync();
            CartCountChanged?.Invoke(this, count);
        }
        return result;
    }

    public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            return await RemoveFromCartAsync(cartItemId);
        }

        var items = await _cartRepository.GetAllCartItemsAsync();
        var item = items.FirstOrDefault(i => i.Id == cartItemId);
        
        if (item == null)
            return false;

        item.Quantity = quantity;
        var result = await _cartRepository.UpdateCartItemAsync(item);
        
        if (result)
        {
            var count = await GetCartItemCountAsync();
            CartCountChanged?.Invoke(this, count);
        }
        return result;
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        var result = await _cartRepository.DeleteCartItemAsync(cartItemId);
        if (result)
        {
            var count = await GetCartItemCountAsync();
            CartCountChanged?.Invoke(this, count);
        }
        return result;
    }

    public async Task<bool> ClearCartAsync()
    {
        var result = await _cartRepository.ClearCartAsync();
        if (result)
        {
            CartCountChanged?.Invoke(this, 0);
        }
        return result;
    }

    public async Task<decimal> GetTotalPriceAsync()
    {
        var items = await _cartRepository.GetAllCartItemsAsync();
        return items.Sum(item => item.TotalPrice);
    }
}
