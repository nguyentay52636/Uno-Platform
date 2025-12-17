using Uno_Platform.Views;

namespace Uno_Platform.Services;

/// <summary>
/// Service hiển thị toast notifications. Singleton pattern.
/// </summary>
public class ToastService
{
    private static ToastService? _instance;

    /// <summary>
    /// Singleton instance của ToastService
    /// </summary>
    public static ToastService Instance
    {
        get
        {
            _instance ??= new ToastService();
            return _instance;
        }
    }

    /// <summary>
    /// Hiển thị toast message thông thường
    /// </summary>
    /// <param name="message">Nội dung message</param>
    /// <param name="duration">Thời gian hiển thị (ms), mặc định 3000ms</param>
    public void ShowMessage(string message, int duration = 3000)
    {
        ShowToastUI(message, true, duration);
    }

    /// <summary>
    /// Hiển thị toast error (với icon X đỏ)
    /// </summary>
    public void ShowError(string message)
    {
        ShowToastUI(message, false, 4000);
    }

    /// <summary>
    /// Hiển thị toast success (với icon ✓ xanh)
    /// </summary>
    public void ShowSuccess(string message)
    {
        ShowToastUI(message, true, 2500);
    }

    /// <summary>
    /// Internal method hiển thị toast qua AppShell UI
    /// </summary>
    private void ShowToastUI(string message, bool isSuccess, int duration)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"TOAST: {message}");
            
            // Show via AppShell if available
            if (AppShell.Instance != null)
            {
                AppShell.Instance.ShowToast(message, isSuccess, duration);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing toast: {ex.Message}");
        }
    }
}
