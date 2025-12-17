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
        ShowToast(message, duration);
    }

    /// <summary>
    /// Hiển thị toast error (với icon ❌)
    /// </summary>
    public void ShowError(string message)
    {
        ShowToast($"❌ {message}", 4000);
    }

    /// <summary>
    /// Hiển thị toast success (với icon ✅)
    /// </summary>
    public void ShowSuccess(string message)
    {
        ShowToast($"✅ {message}", 3000);
    }

    /// <summary>
    /// Internal method hiển thị toast. Hiện tại chỉ log ra Debug console.
    /// TODO: Implement proper toast UI component.
    /// </summary>
    private void ShowToast(string message, int duration)
    {
        try
        {
            // For now, use debug output
            // In a production app, you could use a proper toast notification system
            System.Diagnostics.Debug.WriteLine($"TOAST: {message}");
            
            // Try to show in UI if possible
            _ = Task.Run(async () =>
            {
                await Task.Delay(100);
                // Could implement a proper toast UI component here
                // For now, just log to debug
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing toast: {ex.Message}");
        }
    }
}
