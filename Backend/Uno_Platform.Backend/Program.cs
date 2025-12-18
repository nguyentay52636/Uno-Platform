using Microsoft.EntityFrameworkCore;
using Uno_Platform.Backend.Data;
using Uno_Platform.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "backend.db");
    options.UseSqlite($"Data Source={dbPath}");
});

// Add CORS for mobile app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMobileApp");
app.UseAuthorization();
app.MapControllers();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();
        logger.LogInformation("Database created successfully");
        
        // Seed products if empty
        if (!context.Products.Any())
        {
            logger.LogInformation("Seeding sample products...");
            
            var products = new List<Product>
            {
                new Product { Name = "Gà Rán Giòn", Price = 89000, Category = "Món chính", Description = "Gà rán giòn tan, thơm ngon", Image = "Assets/img/ga_ran_gion.png" },
                new Product { Name = "Gà Rán Nguyên Cánh", Price = 129000, Category = "Món chính", Description = "Gà nguyên cánh rán giòn", Image = "Assets/img/ga_ran_nguyen_canh.png" },
                new Product { Name = "Gà Rán Popcorn", Price = 69000, Category = "Món chính", Description = "Gà rán miếng nhỏ kiểu popcorn", Image = "Assets/img/ga_ran_popcorn.png" },
                new Product { Name = "Hamburger Cổ Điển", Price = 79000, Category = "Món chính", Description = "Hamburger bò phô mai", Image = "Assets/img/hamburger_co_dien.png" },
                new Product { Name = "Coca Cola", Price = 15000, Category = "Đồ uống", Description = "Coca Cola 330ml", Image = "Assets/img/coca_cola.png" },
                new Product { Name = "Pepsi", Price = 15000, Category = "Đồ uống", Description = "Pepsi 330ml", Image = "Assets/img/pepsi.png" },
                new Product { Name = "Khoai Tây Chiên", Price = 29000, Category = "Món phụ", Description = "Khoai tây chiên giòn", Image = "Assets/img/khoai_tay_chien.png" },
                new Product { Name = "Salad Rau", Price = 25000, Category = "Món phụ", Description = "Salad rau tươi", Image = "Assets/img/salad_rau.png" },
                new Product { Name = "Kem Vanilla", Price = 19000, Category = "Tráng miệng", Description = "Kem vanilla mát lạnh", Image = "Assets/img/kem_vanilla.png" },
                new Product { Name = "Bánh Táo", Price = 22000, Category = "Tráng miệng", Description = "Bánh táo nướng", Image = "Assets/img/banh_tao.png" }
            };
            
            context.Products.AddRange(products);
            context.SaveChanges();
            
            logger.LogInformation("Seeded {Count} products successfully", products.Count);
        }
        else
        {
            logger.LogInformation("Database already contains {Count} products", context.Products.Count());
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding database");
    }
}

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Backend API started successfully");
startupLogger.LogInformation("Swagger UI: https://localhost:5001/swagger");
startupLogger.LogInformation("Products API: https://localhost:5001/api/products");
startupLogger.LogInformation("Orders API: https://localhost:5001/api/orders");

app.Run();
