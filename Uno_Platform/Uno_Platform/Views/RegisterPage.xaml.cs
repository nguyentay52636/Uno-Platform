using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Uno_Platform.Views;

public sealed partial class RegisterPage : Page
{
    private bool _isPasswordVisible = false;
    private bool _isConfirmPasswordVisible = false;

    public RegisterPage()
    {
        this.InitializeComponent();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Frame.CanGoBack)
        {
            Frame.GoBack();
        }
        else
        {
            Frame.Navigate(typeof(ProductListPage));
        }
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // Clear previous error message
        ErrorMessageText.Visibility = Visibility.Collapsed;

        // Get form data
        string fullName = FullNameTextBox.Text.Trim();
        string email = EmailTextBox.Text.Trim();
        string username = UsernameTextBox.Text.Trim();
        string password = _isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;
        string confirmPassword = _isConfirmPasswordVisible ? ConfirmPasswordTextBox.Text : ConfirmPasswordBox.Password;

        // Validation
        if (string.IsNullOrEmpty(fullName))
        {
            ShowError("Vui lòng nhập họ và tên");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            ShowError("Vui lòng nhập địa chỉ email");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Địa chỉ email không hợp lệ");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            ShowError("Vui lòng nhập tên đăng nhập");
            return;
        }

        if (username.Length < 3)
        {
            ShowError("Tên đăng nhập phải có ít nhất 3 ký tự");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowError("Vui lòng nhập mật khẩu");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Mật khẩu phải có ít nhất 6 ký tự");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Mật khẩu xác nhận không khớp");
            return;
        }

        // Show loading
        ShowLoading(true);

        try
        {
            // Simulate API call
            await Task.Delay(2000);

            // Show success message
            await ShowSuccessDialog("Thành công!", $"Tài khoản '{username}' đã được tạo thành công.");

            // Navigate to login page
            Frame.Navigate(typeof(LoginPage));
        }
        catch (Exception ex)
        {
            ShowError($"Đăng ký thất bại: {ex.Message}");
        }
        finally
        {
            ShowLoading(false);
        }
    }

    private void LoginLinkButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(LoginPage));
    }

    private void ShowError(string message)
    {
        ErrorMessageText.Text = message;
        ErrorMessageText.Visibility = Visibility.Visible;
    }

    private void ShowLoading(bool isLoading)
    {
        LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        RegisterButton.IsEnabled = !isLoading;
    }

    private async Task ShowSuccessDialog(string title, string message)
    {
        var dialog = new ContentDialog()
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK"
        };

        dialog.XamlRoot = this.XamlRoot;
        await dialog.ShowAsync();
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Toggle Password Visibility
    private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            if (tag == "Password")
            {
                _isPasswordVisible = !_isPasswordVisible;
                
                if (_isPasswordVisible)
                {
                    // Show password as text
                    PasswordTextBox.Text = PasswordBox.Password;
                    PasswordBox.Visibility = Visibility.Collapsed;
                    PasswordTextBox.Visibility = Visibility.Visible;
                    PasswordEyeIcon.Symbol = Symbol.ViewAll; // Eye with line
                }
                else
                {
                    // Hide password
                    PasswordBox.Password = PasswordTextBox.Text;
                    PasswordTextBox.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Visible;
                    PasswordEyeIcon.Symbol = Symbol.View; // Eye
                }
            }
            else if (tag == "ConfirmPassword")
            {
                _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
                
                if (_isConfirmPasswordVisible)
                {
                    // Show confirm password as text
                    ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                    ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                    ConfirmPasswordTextBox.Visibility = Visibility.Visible;
                    ConfirmPasswordEyeIcon.Symbol = Symbol.ViewAll; // Eye with line
                }
                else
                {
                    // Hide confirm password
                    ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
                    ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
                    ConfirmPasswordBox.Visibility = Visibility.Visible;
                    ConfirmPasswordEyeIcon.Symbol = Symbol.View; // Eye
                }
            }
        }
    }
}