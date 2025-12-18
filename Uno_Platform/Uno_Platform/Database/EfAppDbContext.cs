#if !__WASM__
using Microsoft.EntityFrameworkCore;
using System.IO;
using Uno_Platform.Models;
using Windows.Storage;

namespace Uno_Platform.Database;

/// <summary>
/// Entity Framework Core database context cho Products và Cart items. Dùng SQLite làm backend.
/// </summary>
public class EfAppDbContext : DbContext
{
    /// <summary>
    /// DbSet cho Product table
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// DbSet cho CartItem table
    /// </summary>
    public DbSet<CartItem> CartItems { get; set; }

    /// <summary>
    /// Cấu hình EF Core sử dụng SQLite với đường dẫn file tự động theo platform
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string dbPath = GetDatabasePath();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    /// <summary>
    /// Lấy đường dẫn file cart.db theo platform. Tự động tạo thư mục nếu chưa tồn tại.
    /// </summary>
    private string GetDatabasePath()
    {
        string dbPath;
#if __ANDROID__
        // For Android, use local app data
        string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        dbPath = Path.Combine(personalFolder, "unoplatform.db");
#elif __IOS__
        // For iOS, use documents folder
        string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string libraryPath = Path.Combine(documentsPath, "..", "Library");
        dbPath = Path.Combine(libraryPath, "unoplatform.db");
#else
        // For other platforms (Windows, etc.)
        string localFolder = ApplicationData.Current.LocalFolder.Path;
        dbPath = Path.Combine(localFolder, "unoplatform.db");
#endif

        // Ensure the directory exists
        string? directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            System.Diagnostics.Debug.WriteLine($"Created database directory: {directory}");
        }

        System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");
        return dbPath;
    }

    /// <summary>
    /// Cấu hình model: định nghĩa primary key, required fields, data types cho Product và CartItem
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Image).IsRequired();
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure CartItem entity
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ProductName).IsRequired();
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Quantity).IsRequired();
        });
    }
}
#endif

