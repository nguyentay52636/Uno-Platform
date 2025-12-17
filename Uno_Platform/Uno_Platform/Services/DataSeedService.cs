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

            var foodProducts = new List<Product>
            {
                // Món chính - Gà
                new Product
                {
                    Name = "Gà Rán Giòn",
                    Price = 89000m,
                    Description = "Gà rán giòn tan, thơm lừng với lớp vỏ vàng ruộm, thịt gà mềm mọng nước bên trong. Được tẩm ướp gia vị đặc biệt và chiên giòn đến độ hoàn hảo. Món ăn không thể thiếu cho những tín đồ yêu thích đồ chiên!",
                    Category = "Món chính",
                    Image = "Assets/img/chicken.jpg"
                },
                new Product
                {
                    Name = "Gà Rán Nguyên Cánh",
                    Price = 129000m,
                    Description = "Gà rán nguyên cánh với kích thước lớn, thịt gà tươi ngon được tẩm ướp đậm đà và chiên giòn đều. Phần da vàng ươm, giòn rụm, thịt bên trong mềm mại, đậm vị. Thích hợp cho 2-3 người thưởng thức.",
                    Category = "Món chính",
                    Image = "Assets/img/1chicken.jpg"
                },
                new Product
                {
                    Name = "Gà Rán Popcorn",
                    Price = 69000m,
                    Description = "Những miếng gà rán nhỏ xinh như bỏng ngô, giòn tan từng miếng. Thịt gà được cắt nhỏ, tẩm bột đặc biệt và chiên giòn đến độ hoàn hảo. Dễ ăn, dễ thưởng thức, phù hợp cho mọi lứa tuổi. Món ăn vặt lý tưởng cho mọi dịp!",
                    Category = "Món chính",
                    Image = "Assets/img/popcornchicken.jpg"
                },

                // Món chính - Burger
                new Product
                {
                    Name = "Hamburger Cổ Điển",
                    Price = 79000m,
                    Description = "Burger cổ điển với thịt bò tươi ngon, rau xanh tươi mát, cà chua chín đỏ, hành tây giòn và sốt đặc biệt. Bánh mì mềm mại, nướng vàng thơm. Một bữa ăn no nê, đầy đủ dinh dưỡng và hương vị đậm đà khó quên.",
                    Category = "Món chính",
                    Image = "Assets/img/hamburgur.jpg"
                },

                // Món chính - Thịt nướng
                new Product
                {
                    Name = "Thịt Bò Nướng",
                    Price = 159000m,
                    Description = "Thịt bò nướng thơm lừng, mềm mại với lớp sốt BBQ đậm đà. Thịt được ướp gia vị đặc biệt, nướng trên lửa than hoàn hảo, giữ nguyên độ ngọt tự nhiên của thịt. Kèm theo rau sống tươi mát và khoai tây chiên giòn. Món ăn đầy đặn cho bữa trưa hoặc tối.",
                    Category = "Món chính",
                    Image = "Assets/img/beefGrill.jpg"
                },

                // Món chính - Kebab
                new Product
                {
                    Name = "Kebab Thổ Nhĩ Kỳ",
                    Price = 99000m,
                    Description = "Kebab truyền thống với thịt gà/lamb được nướng thơm lừng, cuộn trong bánh mì pita mềm mại. Kèm theo rau sống tươi mát, hành tây, cà chua và sốt đặc biệt. Hương vị đậm đà, đầy đủ chất dinh dưỡng. Món ăn đường phố nổi tiếng thế giới!",
                    Category = "Món chính",
                    Image = "Assets/img/kebap.jpg"
                },

                // Món chính - Burrito
                new Product
                {
                    Name = "Burrito Mexico",
                    Price = 109000m,
                    Description = "Burrito Mexico đậm đà với thịt bò/gà được nấu chín mềm, kèm cơm, đậu, phô mai tan chảy, rau xanh và sốt salsa cay nồng. Tất cả được cuộn trong bánh tortilla mềm mại. Một bữa ăn no nê, đầy hương vị Mexico chính thống!",
                    Category = "Món chính",
                    Image = "Assets/img/burrito.jpg"
                },

                // Món chính - Xúc xích
                new Product
                {
                    Name = "Xúc Xích Nướng",
                    Price = 59000m,
                    Description = "Xúc xích nướng thơm lừng, giòn tan bên ngoài, mềm mại bên trong. Được làm từ thịt tươi ngon, gia vị đậm đà. Kèm theo bánh mì mềm, sốt cà chua, mù tạt và rau sống. Món ăn nhanh, tiện lợi và đầy hương vị!",
                    Category = "Món chính",
                    Image = "Assets/img/sausage.jpg"
                },

                // Món chính - Spaghetti
                new Product
                {
                    Name = "Spaghetti Sốt Thịt Bò",
                    Price = 119000m,
                    Description = "Spaghetti Ý chính thống với sốt thịt bò đậm đà, phô mai Parmesan béo ngậy. Mì spaghetti được nấu al dente hoàn hảo, sốt thịt bò được nấu từ thịt bò tươi, cà chua chín đỏ và các loại gia vị đặc biệt. Món ăn Ý cổ điển, đầy hương vị!",
                    Category = "Món chính",
                    Image = "Assets/img/spagheti.jpg"
                },

                // Món phụ
                new Product
                {
                    Name = "Khoai Tây Chiên",
                    Price = 39000m,
                    Description = "Khoai tây chiên giòn tan, vàng ruộm, nóng hổi vừa ra lò. Được cắt từ khoai tây tươi ngon, chiên giòn đến độ hoàn hảo. Kèm theo sốt cà chua hoặc sốt mayonnaise. Món ăn kèm không thể thiếu, giòn tan từng miếng!",
                    Category = "Món phụ",
                    Image = "Assets/img/fries.jpg"
                },
                new Product
                {
                    Name = "Bỏng Ngô Caramel",
                    Price = 49000m,
                    Description = "Bỏng ngô ngọt ngào với lớp caramel vàng óng, giòn tan từng hạt. Được làm từ ngô tươi, rang thơm lừng và phủ lớp caramel đậm đà. Món ăn vặt hoàn hảo cho mọi dịp, từ xem phim đến tụ tập bạn bè!",
                    Category = "Món phụ",
                    Image = "Assets/img/popcorn.jpg"
                },

                // Đồ uống
                new Product
                {
                    Name = "Pepsi",
                    Price = 25000m,
                    Description = "Pepsi tươi mát, có ga, giải khát hoàn hảo. Vị ngọt đậm đà, sảng khoái, thích hợp để kèm với các món chiên. Làm mát lạnh, uống ngay là thích nhất!",
                    Category = "Đồ uống",
                    Image = "Assets/img/pepsi.jpg"
                },
                new Product
                {
                    Name = "Sprite",
                    Price = 25000m,
                    Description = "Sprite chanh tươi mát, có ga, giải khát sảng khoái. Vị chanh chua ngọt thanh mát, không quá ngọt. Thích hợp cho mọi lứa tuổi, đặc biệt hoàn hảo khi kèm với đồ chiên nóng!",
                    Category = "Đồ uống",
                    Image = "Assets/img/sprite.jpg"
                }
            };

            foreach (var product in foodProducts)
            {
                await _productRepository.AddProductAsync(product);
            }

            System.Diagnostics.Debug.WriteLine($"✅ Successfully seeded {foodProducts.Count} food products");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error seeding data: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to see the error in app
        }
    }
}
