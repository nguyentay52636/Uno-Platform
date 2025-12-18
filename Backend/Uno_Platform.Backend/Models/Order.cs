using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uno_Platform.Backend.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string CustomerPhone { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string CustomerAddress { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Note { get; set; } = string.Empty;
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation property
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    public decimal ProductPrice { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    // Navigation property
    public Order Order { get; set; } = null!;
}

