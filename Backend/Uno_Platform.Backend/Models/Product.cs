using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uno_Platform.Backend.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Image { get; set; } = "Assets/img/caby.png";
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

