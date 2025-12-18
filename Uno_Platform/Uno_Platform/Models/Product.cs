using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uno_Platform.Models;

/// <summary>
/// Model cho sản phẩm. Lưu trong SQLite (Android/Windows) hoặc InMemory (WASM).
/// </summary>
public class Product
{
    /// <summary>ID tự động tăng</summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>Tên sản phẩm</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Giá sản phẩm (decimal cho độ chính xác)</summary>
    public decimal Price { get; set; }
    
    /// <summary>Mô tả chi tiết sản phẩm</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>Đường dẫn ảnh (luôn dùng ảnh mặc định)</summary>
    public string Image { get; set; } = "Assets/img/caby.png";
    
    /// <summary>Danh mục sản phẩm (Electronics, Food, etc.)</summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>Thời gian tạo sản phẩm</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
