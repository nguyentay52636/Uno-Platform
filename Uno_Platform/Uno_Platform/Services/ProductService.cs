using Uno_Platform.Models;
using Uno_Platform.Repositories;

namespace Uno_Platform.Services;

/// <summary>
/// Service layer cho Product operations. Xử lý business logic và validation.
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lấy tất cả sản phẩm từ database
    /// </summary>
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllProductsAsync();
    }

    /// <summary>
    /// Tìm sản phẩm theo ID. Returns null nếu không tìm thấy.
    /// </summary>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repository.GetProductByIdAsync(id);
    }

    /// <summary>
    /// Tìm kiếm sản phẩm theo từ khóa (tên hoặc mô tả). Returns tất cả nếu keyword rỗng.
    /// </summary>
    public async Task<List<Product>> SearchProductsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return await GetAllProductsAsync();
        
        return await _repository.SearchProductsAsync(keyword);
    }

    /// <summary>
    /// Lấy sản phẩm theo category. Returns tất cả nếu category rỗng.
    /// </summary>
    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return await GetAllProductsAsync();
        
        return await _repository.GetProductsByCategoryAsync(category);
    }

    /// <summary>
    /// Lấy sản phẩm trong khoảng giá từ minPrice đến maxPrice
    /// </summary>
    public async Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _repository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
    }

    /// <summary>
    /// Thêm sản phẩm mới. Tự động validate trước khi thêm. Returns false nếu validation fail.
    /// </summary>
    public async Task<bool> AddProductAsync(Product product)
    {
        if (!ValidateProduct(product))
            return false;
        
        return await _repository.AddProductAsync(product);
    }

    /// <summary>
    /// Cập nhật sản phẩm. Tự động validate trước khi update. Returns false nếu validation fail.
    /// </summary>
    public async Task<bool> UpdateProductAsync(Product product)
    {
        if (!ValidateProduct(product))
            return false;
        
        return await _repository.UpdateProductAsync(product);
    }

    /// <summary>
    /// Xóa sản phẩm theo ID
    /// </summary>
    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _repository.DeleteProductAsync(id);
    }

    /// <summary>
    /// Lấy danh sách tất cả categories (unique)
    /// </summary>
    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _repository.GetAllCategoriesAsync();
    }

    /// <summary>
    /// Validate sản phẩm: kiểm tra tên (>= 2 ký tự), giá (>= 0), description và category không rỗng
    /// </summary>
    private bool ValidateProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.Name) || product.Name.Length < 2)
            return false;
        
        if (product.Price < 0)
            return false;
        
        if (string.IsNullOrWhiteSpace(product.Description))
            return false;
        
        if (string.IsNullOrWhiteSpace(product.Category))
            return false;
        
        return true;
    }
}
