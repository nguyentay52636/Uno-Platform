using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uno_Platform.Backend.Data;
using Uno_Platform.Backend.Models;

namespace Uno_Platform.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(AppDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// GET /api/products - Lấy tất cả sản phẩm
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        try
        {
            var products = await _context.Products
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToListAsync();
            
            _logger.LogInformation("Retrieved {Count} products", products.Count);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// <summary>
    /// GET /api/products/{id} - Lấy sản phẩm theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }
            
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// <summary>
    /// GET /api/products/category/{category} - Lấy sản phẩm theo category
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<Product>>> GetProductsByCategory(string category)
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
            
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by category {Category}", category);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

