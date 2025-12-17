using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Uno_Platform.Services;

/// <summary>
/// Service điều hướng giữa các pages trong app. Wrapper cho Frame navigation.
/// </summary>
public class NavigationService
{
    private Frame? _frame;

    /// <summary>
    /// Khởi tạo service với Frame từ AppShell
    /// </summary>
    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    /// <summary>
    /// Điều hướng đến page mới. Returns false nếu frame chưa được initialize.
    /// </summary>
    /// <param name="pageType">Type của page (typeof(HomePage), typeof(ProductPage), etc.)</param>
    /// <param name="parameter">Optional parameter truyền cho page</param>
    public bool NavigateTo(Type pageType, object? parameter = null)
    {
        if (_frame == null)
        {
            return false;
        }

        return _frame.Navigate(pageType, parameter);
    }

    /// <summary>
    /// Kiểm tra có thể quay lại page trước không
    /// </summary>
    public bool CanGoBack()
    {
        return _frame?.CanGoBack ?? false;
    }

    /// <summary>
    /// Quay lại page trước (nếu có)
    /// </summary>
    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }

    /// <summary>
    /// Xóa toàn bộ back stack (dùng khi logout hoặc reset navigation)
    /// </summary>
    public void ClearBackStack()
    {
        if (_frame != null)
        {
            while (_frame.CanGoBack)
            {
                _frame.BackStack.RemoveAt(_frame.BackStack.Count - 1);
            }
        }
    }
}
