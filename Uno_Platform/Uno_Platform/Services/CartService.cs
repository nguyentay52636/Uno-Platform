using Uno_Platform.Models;
using Uno_Platform.Repositories;

namespace Uno_Platform.Services;

/// <summary>
/// Service layer cho Cart operations. Quản lý giỏ hàng và trigger CartCountChanged event.
/// </summary>
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Event được trigger khi số lượng items trong cart thay đổi. Dùng để update UI badge.
    /// </summary>
    public event EventHandler<int>? CartCountChanged;

    /// <summary>
    /// Lấy tất cả items trong giỏ hàng
    /// </summary>
    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        return await _cartRepository.GetAllCartItemsAsync();
    }

    /// <summary>
    /// Lấy tổng số items trong giỏ hàng (sum của tất cả quantities)
    /// </summary>
    public async Task<int> GetCartItemCountAsync()
    {
        return await _cartRepository.GetCartItemCountAsync();
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng. Nếu đã tồn tại thì tăng quantity lên 1. Trigger CartCountChanged event.
    /// </summary>
    /// <param name="productId">ID của sản phẩm cần thêm</param>
    /// <returns>True nếu thêm thành công, false nếu product không tồn tại</returns>
    public async Task<bool> AddToCartAsync(int productId)
    {
        var product = await _productRepository.GetProductByIdAsync(productId);
        if (product == null)
        {
            System.Diagnostics.Debug.WriteLine($"=== CartService: Product {productId} not found ===");
            return false;
        }
        System.Diagnostics.Debug.WriteLine($"=== CartService: Adding {product.Name} Price={product.Price} to cart ===");

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
                ProductImage = product.Image ?? "Assets/img/caby.png", // Use product image if available
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

    /// <summary>
    /// Cập nhật số lượng của cart item. Nếu quantity <= 0 thì xóa item khỏi cart. Trigger CartCountChanged event.
    /// </summary>
    /// <param name="cartItemId">ID của cart item (không phải product ID)</param>
    /// <param name="quantity">Số lượng mới</param>
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

    /// <summary>
    /// Xóa item khỏi giỏ hàng. Trigger CartCountChanged event.
    /// </summary>
    /// <param name="cartItemId">ID của cart item (không phải product ID)</param>
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

    /// <summary>
    /// Xóa toàn bộ giỏ hàng (thường dùng sau khi checkout thành công). Trigger CartCountChanged với count = 0.
    /// </summary>
    public async Task<bool> ClearCartAsync()
    {
        var result = await _cartRepository.ClearCartAsync();
        if (result)
        {
            CartCountChanged?.Invoke(this, 0);
        }
        return result;
    }

    /// <summary>
    /// Tính tổng giá trị giỏ hàng (sum của tất cả TotalPrice = ProductPrice * Quantity)
    /// </summary>
    public async Task<decimal> GetTotalPriceAsync()
    {
        var items = await _cartRepository.GetAllCartItemsAsync();
        return items.Sum(item => item.TotalPrice);
    }

    /// <summary>
    /// Cập nhật thông tin product trong cart khi product bị sửa (đồng bộ tên, giá, category)
    /// </summary>
    public async Task UpdateProductInCartAsync(Product product)
    {
        var item = await _cartRepository.GetCartItemByProductIdAsync(product.Id);
        if (item != null)
        {
            item.ProductName = product.Name;
            item.ProductPrice = product.Price;
            item.ProductImage = product.Image ?? "Assets/img/caby.png"; // Update image from product
            item.ProductCategory = product.Category;
            
            await _cartRepository.UpdateCartItemAsync(item);
            System.Diagnostics.Debug.WriteLine($"=== CartService: Updated cart item for product {product.Name} ===");
        }
    }
}
