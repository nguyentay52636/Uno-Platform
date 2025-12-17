#if !__WASM__
using SQLite;
using System.IO;
using Windows.Storage;
using Uno_Platform.Models;

namespace Uno_Platform.Database;

/// <summary>
/// SQLite database context cho Android/iOS/Windows. Data lưu persistent trên disk.
/// </summary>
public class AppDbContext
{
    private SQLiteConnection? _connection;
    private const string DatabaseFileName = "unoplatform.db";

    /// <summary>
    /// SQLite connection. Tự động khởi tạo database nếu chưa tồn tại.
    /// </summary>
    public SQLiteConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                InitializeDatabase();
            }
            return _connection!;
        }
    }

    /// <summary>
    /// Khởi tạo database: mở connection và tạo tables
    /// </summary>
    private void InitializeDatabase()
    {
        string dbPath = GetDatabasePath();
        _connection = new SQLiteConnection(dbPath);
        CreateTables();
    }

    /// <summary>
    /// Lấy đường dẫn file database theo platform (Android: /data/data/.../files/, Windows: AppData/Local)
    /// </summary>
    private string GetDatabasePath()
    {
#if __ANDROID__
        // For Android, use local app data
        string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        return Path.Combine(personalFolder, DatabaseFileName);
#else
        // For other platforms
        string localFolder = ApplicationData.Current.LocalFolder.Path;
        return Path.Combine(localFolder, DatabaseFileName);
#endif
    }

    /// <summary>
    /// Tạo tables Product và CartItem nếu chưa tồn tại
    /// </summary>
    private void CreateTables()
    {
        Connection.CreateTable<Product>();
        Connection.CreateTable<CartItem>();
    }

    /// <summary>
    /// Đóng connection và giải phóng resources
    /// </summary>
    public void CloseConnection()
    {
        _connection?.Close();
        _connection = null;
    }
}
#endif

