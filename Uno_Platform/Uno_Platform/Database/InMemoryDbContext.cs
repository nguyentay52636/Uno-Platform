using Uno_Platform.Models;

namespace Uno_Platform.Database;

/// <summary>
/// Database context lưu trữ data trong RAM (chỉ dùng cho WebAssembly). Data sẽ mất khi refresh browser.
/// </summary>
public class InMemoryDbContext
{
    private readonly List<Product> _products = new();
    private readonly List<CartItem> _cartItems = new();
    private int _nextProductId = 1;
    private int _nextCartItemId = 1;

    /// <summary>
    /// Lấy tất cả sản phẩm từ RAM
    /// </summary>
    public List<Product> GetAllProducts() => _products.ToList();

    /// <summary>
    /// Tìm sản phẩm theo ID
    /// </summary>
    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);

    /// <summary>
    /// Thêm sản phẩm mới vào RAM. Tự động gán ID và ảnh mặc định.
    /// </summary>
    public void AddProduct(Product product)
    {
        product.Id = _nextProductId++;
        product.Image = "Assets/img/caby.png"; // Always use default image
        _products.Add(product);
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm. Giữ nguyên ngày tạo ban đầu.
    /// </summary>
    public void UpdateProduct(Product product)
    {
        product.Image = "Assets/img/caby.png"; // Always use default image
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            // Create a new instance to avoid reference issues
            _products[index] = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
                Image = "Assets/img/caby.png",
                CreatedAt = _products[index].CreatedAt // Preserve original creation date
            };
        }
    }

    /// <summary>
    /// Xóa sản phẩm theo ID
    /// </summary>
    public void DeleteProduct(int id)
    {
        _products.RemoveAll(p => p.Id == id);
    }

    /// <summary>
    /// Lấy tất cả items trong giỏ hàng
    /// </summary>
    public List<CartItem> GetAllCartItems() => _cartItems.ToList();

    /// <summary>
    /// Tìm item trong giỏ hàng theo Product ID
    /// </summary>
    public CartItem? GetCartItemByProductId(int productId) => _cartItems.FirstOrDefault(c => c.ProductId == productId);

    /// <summary>
    /// Thêm item vào giỏ hàng. Tự động gán ID và ảnh mặc định.
    /// </summary>
    public void AddCartItem(CartItem item)
    {
        item.Id = _nextCartItemId++;
        item.ProductImage = "Assets/img/caby.png"; // Always use default image
        _cartItems.Add(item);
    }

    /// <summary>
    /// Cập nhật số lượng hoặc thông tin item trong giỏ hàng
    /// </summary>
    public void UpdateCartItem(CartItem item)
    {
        item.ProductImage = "Assets/img/caby.png"; // Always use default image
        var index = _cartItems.FindIndex(c => c.Id == item.Id);
        if (index >= 0)
        {
            _cartItems[index] = item;
        }
    }

    /// <summary>
    /// Xóa item khỏi giỏ hàng theo ID
    /// </summary>
    public void DeleteCartItem(int id)
    {
        _cartItems.RemoveAll(c => c.Id == id);
    }

    /// <summary>
    /// Xóa toàn bộ giỏ hàng (sau khi checkout thành công)
    /// </summary>
    public void ClearCart()
    {
        _cartItems.Clear();
    }
}
