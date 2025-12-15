# 🛍️ Uno Platform Retail Store

[![Uno Platform](https://img.shields.io/badge/Uno%20Platform-5.1-663399?style=for-the-badge&logo=microsoft)]()
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)]()
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)]()
[![XAML](https://img.shields.io/badge/XAML-0C54C2?style=for-the-badge&logo=xaml)]()
[![License: MIT](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)]()

---

## 🚀 Giới thiệu

**Uno Platform Retail Store** là ứng dụng bán hàng đa nền tảng hiện đại được xây dựng trên **Uno Platform**, cho phép chạy trên **Android, iOS, Windows, Web** và **macOS** từ cùng một codebase C#.

Ứng dụng cung cấp trải nghiệm mua sắm mượt mà với giao diện người dùng đẹp mắt và hiệu suất cao trên mọi thiết bị.

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|------------|-----------|
| **Framework** | Uno Platform 5.1+ |
| **Backend** | .NET 8.0, C# 12 |
| **Database** | SQLite (Local Storage) |
| **UI** | XAML, WinUI 3 Controls |
| **Architecture** | MVVM Pattern |
| **Platforms** | Android, iOS, Windows, WebAssembly, macOS |

---

## ✨ Tính năng chính

### 🛒 Quản lý sản phẩm
- Hiển thị danh sách sản phẩm với hình ảnh
- Tìm kiếm và lọc sản phẩm theo danh mục
- Chi tiết sản phẩm với mô tả đầy đủ
- Quản lý kho hàng và giá cả

### 🛍️ Giỏ hàng thông minh
- Thêm/xóa sản phẩm vào giỏ hàng
- Cập nhật số lượng sản phẩm
- Tính toán tổng tiền tự động
- Lưu trữ giỏ hàng offline

### 👤 Quản lý người dùng
- Đăng nhập/đăng ký tài khoản
- Hồ sơ người dùng cá nhân
- Lịch sử đơn hàng
- Địa chỉ giao hàng

### 📊 Báo cáo & Thống kê
- Thống kê doanh thu theo thời gian
- Sản phẩm bán chạy nhất
- Báo cáo tồn kho
- Phân tích khách hàng

---

## 🏗️ Kiến trúc dự án

```
📦 Uno_Platform
┣ 📂 Uno_Platform/
┃ ┣ 📂 Assets/           # Hình ảnh, icons, resources
┃ ┣ 📂 Components/       # UI Components tái sử dụng
┃ ┣ 📂 Converters/       # XAML Value Converters
┃ ┣ 📂 Database/         # SQLite Database Context
┃ ┣ 📂 Models/           # Data Models
┃ ┣ 📂 Repositories/     # Data Access Layer
┃ ┣ 📂 Services/         # Business Logic Services
┃ ┣ 📂 ViewModels/       # MVVM ViewModels
┃ ┣ 📂 Views/            # XAML Pages & Controls
┃ ┣ 📂 Platforms/        # Platform-specific code
┃ ┗ 📂 Themes/           # UI Themes & Styles
┣ 📂 docs/               # Tài liệu dự án
┗ 📜 README.md
```

---

## 🎯 Nền tảng hỗ trợ

| Platform | Status | Mô tả |
|----------|--------|-------|
| 🤖 **Android** | ✅ Supported | Android 7.0+ (API 24+) |
| 🍎 **iOS** | ✅ Supported | iOS 12.0+ |
| 🪟 **Windows** | ✅ Supported | Windows 10 1903+ |
| 🌐 **WebAssembly** | ✅ Supported | Modern browsers |
| 🍎 **macOS** | ✅ Supported | macOS 10.15+ |

---

## 🗄️ Cơ sở dữ liệu

Ứng dụng sử dụng **SQLite** để lưu trữ dữ liệu local với các bảng chính:

- **Products**: Thông tin sản phẩm
- **CartItems**: Giỏ hàng người dùng
- **Users**: Tài khoản người dùng
- **Orders**: Đơn hàng
- **Categories**: Danh mục sản phẩm

```csharp
// Database Context Example
public class AppDbContext
{
    private SQLiteConnection? _connection;
    private const string DatabaseFileName = "unoplatform.db";
    
    public SQLiteConnection Connection { get; }
    
    private void CreateTables()
    {
        Connection.CreateTable<Product>();
        Connection.CreateTable<CartItem>();
    }
}
```

---

## 🚀 Cài đặt & Chạy dự án

### Yêu cầu hệ thống
- Visual Studio 2022 17.8+ hoặc VS Code
- .NET 8.0 SDK
- Uno Platform Project Templates

### Cài đặt
```bash
# Clone repository
git clone https://github.com/your-username/uno-platform-retail-store.git
cd uno-platform-retail-store

# Restore packages
dotnet restore

# Build solution
dotnet build
```

### Chạy trên các nền tảng
```bash
# Android
dotnet run --project Uno_Platform --framework net9.0-android
# Build for Android (this should work)
dotnet build -f net9.0-android -t:Run


# Try to run/deploy (if you have an Android emulator or device connected)
dotnet build -f net9.0-android -t:Run

# Build for WebAssembly (alternative for testing)
dotnet run -f net9.0-browserwasm

```

---

## 📱 Screenshots

| Android | iOS | Windows |
|---------|-----|---------|
| ![Android](docs/screenshots/android.png) | ![iOS](docs/screenshots/ios.png) | ![Windows](docs/screenshots/windows.png) |



## 👥 Tác giả

- **Your Name** - *Initial work* - [YourGitHub](https://github.com/nguyentay52636)

---

## 🙏 Acknowledgments

- [Uno Platform](https://platform.uno/) - Amazing cross-platform framework
- [Microsoft](https://microsoft.com/) - .NET ecosystem
- [SQLite](https://sqlite.org/) - Reliable database engine