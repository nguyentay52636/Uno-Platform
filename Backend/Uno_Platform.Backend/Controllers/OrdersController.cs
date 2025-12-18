using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uno_Platform.Backend.Data;
using Uno_Platform.Backend.Models;

namespace Uno_Platform.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrdersController> _logger;
    
    public OrdersController(AppDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// POST /api/orders - Tạo đơn hàng mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(new { message = "Customer name is required" });
            }
            
            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
            {
                return BadRequest(new { message = "Customer phone is required" });
            }
            
            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
            {
                return BadRequest(new { message = "Customer address is required" });
            }
            
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { message = "Order must contain at least one item" });
            }
            
            // Create order
            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                Note = request.Note ?? string.Empty,
                TotalPrice = request.TotalPrice,
                CreatedAt = DateTime.Now,
                Items = request.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerName}", 
                order.Id, order.CustomerName);
            
            return Ok(new 
            { 
                message = "Order created successfully", 
                orderId = order.Id,
                totalPrice = order.TotalPrice
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    /// <summary>
    /// GET /api/orders - Lấy tất cả đơn hàng
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders()
    {
        try
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// DTO for order request
public class OrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string? Note { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

