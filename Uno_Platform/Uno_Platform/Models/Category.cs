namespace Uno_Platform.Models;

/// <summary>
/// Model cho danh mục sản phẩm
/// </summary>
public class Category
{
    /// <summary>ID danh mục</summary>
    public int Id { get; set; }
    
    /// <summary>Tên danh mục (Electronics, Food, Clothing, etc.)</summary>
    public string Name { get; set; } = string.Empty;
}
