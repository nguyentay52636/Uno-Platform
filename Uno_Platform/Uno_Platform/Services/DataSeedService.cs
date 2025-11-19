using Uno_Platform.Models;
using Uno_Platform.Repositories;

namespace Uno_Platform.Services;

public class DataSeedService
{
    private readonly IProductRepository _productRepository;

    public DataSeedService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task SeedDataAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== Starting SeedDataAsync ===");
            
            var existingProducts = await _productRepository.GetAllProductsAsync();
            System.Diagnostics.Debug.WriteLine($"Existing products count: {existingProducts.Count}");
            
            if (existingProducts.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Data already seeded, skipping...");
                return; // Data already seeded
            }

            System.Diagnostics.Debug.WriteLine("No existing products, starting seed...");

            var technologyProducts = new List<Product>
            {
                // Smartphones
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Price = 999.00m,
                    Description = "Latest iPhone with A17 Pro chip, titanium design, and advanced camera system",
                    Category = "Smartphones",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Samsung Galaxy S24 Ultra",
                    Price = 1199.00m,
                    Description = "Premium Android flagship with S Pen, 200MP camera, and Snapdragon 8 Gen 3",
                    Category = "Smartphones",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Google Pixel 8 Pro",
                    Price = 899.00m,
                    Description = "AI-powered smartphone with exceptional camera and Google Tensor G3",
                    Category = "Smartphones",
                    Image = "Assets/img/caby.png"
                },

                // Laptops
                new Product
                {
                    Name = "MacBook Pro 16-inch",
                    Price = 2499.00m,
                    Description = "Powerful laptop with M3 Max chip, Liquid Retina XDR display, and up to 22 hours battery",
                    Category = "Laptops",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Dell XPS 15",
                    Price = 1499.00m,
                    Description = "Premium Windows laptop with Intel Core i7, 4K OLED display, and NVIDIA RTX graphics",
                    Category = "Laptops",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Lenovo ThinkPad X1 Carbon",
                    Price = 1299.00m,
                    Description = "Business ultrabook with Intel Core i7, 14-inch display, and exceptional keyboard",
                    Category = "Laptops",
                    Image = "Assets/img/caby.png"
                },

                // Headphones
                new Product
                {
                    Name = "Sony WH-1000XM5",
                    Price = 399.00m,
                    Description = "Industry-leading noise cancellation headphones with 30-hour battery and premium sound",
                    Category = "Headphones",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "AirPods Pro 2",
                    Price = 249.00m,
                    Description = "Apple's premium earbuds with active noise cancellation and spatial audio",
                    Category = "Headphones",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Bose QuietComfort 45",
                    Price = 329.00m,
                    Description = "Comfortable over-ear headphones with excellent noise cancellation",
                    Category = "Headphones",
                    Image = "Assets/img/caby.png"
                },

                // Tablets
                new Product
                {
                    Name = "iPad Pro 12.9-inch",
                    Price = 1099.00m,
                    Description = "Powerful tablet with M2 chip, Liquid Retina XDR display, and Apple Pencil support",
                    Category = "Tablets",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Samsung Galaxy Tab S9 Ultra",
                    Price = 1199.00m,
                    Description = "Large Android tablet with S Pen, Snapdragon 8 Gen 2, and stunning AMOLED display",
                    Category = "Tablets",
                    Image = "Assets/img/caby.png"
                },

                // Smartwatches
                new Product
                {
                    Name = "Apple Watch Series 9",
                    Price = 399.00m,
                    Description = "Advanced smartwatch with S9 SiP, health tracking, and always-on display",
                    Category = "Smartwatches",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Samsung Galaxy Watch 6 Classic",
                    Price = 349.00m,
                    Description = "Premium smartwatch with rotating bezel, health monitoring, and long battery life",
                    Category = "Smartwatches",
                    Image = "Assets/img/caby.png"
                },

                // Gaming
                new Product
                {
                    Name = "PlayStation 5",
                    Price = 499.00m,
                    Description = "Next-gen gaming console with ray tracing, 4K gaming, and ultra-fast SSD",
                    Category = "Gaming",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Xbox Series X",
                    Price = 499.00m,
                    Description = "Powerful gaming console with 4K gaming, ray tracing, and Game Pass",
                    Category = "Gaming",
                    Image = "Assets/img/caby.png"
                },

                // Accessories
                new Product
                {
                    Name = "Logitech MX Master 3S",
                    Price = 99.00m,
                    Description = "Premium wireless mouse with precision tracking and ergonomic design",
                    Category = "Accessories",
                    Image = "Assets/img/caby.png"
                },
                new Product
                {
                    Name = "Keychron K8 Pro",
                    Price = 89.00m,
                    Description = "Mechanical keyboard with hot-swappable switches and RGB backlighting",
                    Category = "Accessories",
                    Image = "Assets/img/caby.png"
                }
            };

            foreach (var product in technologyProducts)
            {
                await _productRepository.AddProductAsync(product);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Successfully seeded {technologyProducts.Count} technology products");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error seeding data: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to see the error in app
        }
    }
}
