using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uno_Platform.Models;

/// <summary>
/// Model cho item trong giỏ hàng. Lưu trong EF Core SQLite (Android/Windows) hoặc InMemory (WASM).
/// </summary>
public class CartItem
{
    /// <summary>ID tự động tăng của cart item</summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>ID của sản phẩm gốc (foreign key)</summary>
    public int ProductId { get; set; }
    
    /// <summary>Tên sản phẩm (denormalized để tránh join)</summary>
    public string ProductName { get; set; } = string.Empty;
    
    /// <summary>Giá sản phẩm tại thời điểm thêm vào cart</summary>
    public decimal ProductPrice { get; set; }
    
    /// <summary>Đường dẫn ảnh sản phẩm</summary>
    public string ProductImage { get; set; } = "Assets/img/caby.png";
    
    /// <summary>Danh mục sản phẩm</summary>
    public string ProductCategory { get; set; } = string.Empty;
    
    /// <summary>Số lượng sản phẩm trong cart</summary>
    public int Quantity { get; set; } = 1;
    
    /// <summary>Thời gian thêm vào cart</summary>
    public DateTime AddedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Tổng giá = ProductPrice * Quantity (computed property)
    /// </summary>
    public decimal TotalPrice => ProductPrice * Quantity;
}
