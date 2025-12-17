using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Uno_Platform.Services;

namespace Uno_Platform.Views;

public sealed partial class AppShell : Page
{
    private string _currentTab = "Home";
    private bool _isSidebarOpen = false;
    private System.Threading.Timer? _toastTimer;
    
    public static AppShell? Instance { get; private set; }

    public AppShell()
    {
        this.InitializeComponent();
        Instance = this;
        this.Loaded += AppShell_Loaded;
        InitializeNavigation();
    }

    private void AppShell_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Initialize Cart service after page is loaded (database is ready)
        _ = InitializeCartServiceAsync();
    }

    private async Task InitializeCartServiceAsync()
    {
        try
        {
            // Subscribe to Cart events
            var cartService = ServiceLocator.CartService;
            cartService.CartCountChanged += (s, count) =>
            {
                DispatcherQueue.TryEnqueue(() => UpdateCartBadge(count));
            };

            // Initialize badge
            var count = await cartService.GetCartItemCountAsync();
            DispatcherQueue.TryEnqueue(() => UpdateCartBadge(count));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing cart service: {ex.Message}");
        }
    }

    private void InitializeNavigation()
    {
        // Listen to frame navigation to update header/tabbar visibility and tab indicators
        ContentFrame.Navigated += (s, e) => 
        {
            UpdateHeaderVisibility();
            UpdateTabBarVisibility();
            SyncTabIndicatorsWithCurrentPage();
        };

        // Check authentication (Optional - removed mandatory check)
        // var authService = new AuthenticationService();
        // if (authService.IsAuthenticated) ...
        
        // Always start at Home
        NavigateToHome();
    }

    private void SyncTabIndicatorsWithCurrentPage()
    {
        // Sync tab indicators based on current page type
        if (ContentFrame.Content is ProductListPage)
        {
            _currentTab = "Home";
            UpdateTabIndicators("Home");
        }
        else if (ContentFrame.Content is CartPage)
        {
            _currentTab = "Cart";
            UpdateTabIndicators("Cart");
        }
        else if (ContentFrame.Content is SettingsPage)
        {
            _currentTab = "Settings";
            UpdateTabIndicators("Settings");
        }
    }

    private void HeaderLoginButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CloseSidebar();
        NavigateToLogin();
    }

    private void HeaderRegisterButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CloseSidebar();
        NavigateToRegister();
    }

    // Sidebar Toggle Button Click
    private void SidebarToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (_isSidebarOpen)
        {
            CloseSidebar();
        }
        else
        {
            OpenSidebar();
        }
    }

    // Sidebar Overlay Tapped (close sidebar)
    private void SidebarOverlay_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        CloseSidebar();
    }

    private void OpenSidebar()
    {
        _isSidebarOpen = true;
        SidebarOverlay.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        
        // Animate sidebar slide in
        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        var animation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = -280,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new Microsoft.UI.Xaml.Media.Animation.CubicEase()
            {
                EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut
            }
        };
        
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(animation, SidebarTransform);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(animation, "X");
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private void CloseSidebar()
    {
        if (!_isSidebarOpen) return;
        
        _isSidebarOpen = false;
        
        // Animate sidebar slide out
        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        var animation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = 0,
            To = -280,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new Microsoft.UI.Xaml.Media.Animation.CubicEase()
            {
                EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseIn
            }
        };
        
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(animation, SidebarTransform);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(animation, "X");
        storyboard.Children.Add(animation);
        
        storyboard.Completed += (s, e) =>
        {
            SidebarOverlay.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        };
        
        storyboard.Begin();
    }

    private void NavigateToRegister()
    {
        // Navigate to Register page
        ContentFrame.Navigate(typeof(RegisterPage));
        PlayPageTransition();
    }

    private void NavigateToLogin()
    {
        // Navigate to Login page
        ContentFrame.Navigate(typeof(LoginPage));
        PlayPageTransition();
    }

    private void NavigateToHome()
    {
        if (_currentTab == "Home" && ContentFrame.Content is ProductListPage) return;
        
        _currentTab = "Home";
        if (!(ContentFrame.Content is ProductListPage))
        {
            ContentFrame.Navigate(typeof(ProductListPage));
        }
        UpdateTabIndicators("Home");
        // Visibility updates handled by Navigated event
        PlayPageTransition();
    }

    private void NavigateToCart()
    {
        if (_currentTab == "Cart" && ContentFrame.Content is CartPage) return;
        
        _currentTab = "Cart";
        if (!(ContentFrame.Content is CartPage))
        {
            ContentFrame.Navigate(typeof(CartPage));
        }
        UpdateTabIndicators("Cart");
        // Visibility updates handled by Navigated event
        PlayPageTransition();
    }

    private void NavigateToSettings()
    {
        if (_currentTab == "Settings" && ContentFrame.Content is SettingsPage) return;
        
        _currentTab = "Settings";
        if (!(ContentFrame.Content is SettingsPage))
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }
        UpdateTabIndicators("Settings");
        // Visibility updates handled by Navigated event
        PlayPageTransition();
    }

    private void UpdateHeaderVisibility()
    {
        // Show header only on Home page (ProductListPage)
        // Hide on Login/Register pages
        if (ContentFrame.Content is LoginPage || ContentFrame.Content is RegisterPage)
        {
            AppBarBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        else if (ContentFrame.Content is ProductListPage)
        {
            AppBarBorder.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            AppBarBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void UpdateTabBarVisibility()
    {
        // Hide TabBar on Login/Register pages
        if (ContentFrame.Content is LoginPage || ContentFrame.Content is RegisterPage)
        {
            TabBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        else
        {
            TabBar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
    }

    private void UpdateTabIndicators(string activeTab)
    {
        // Reset all indicators
        HomeIndicator.Opacity = 0;
        CartIndicator.Opacity = 0;
        SettingsIndicator.Opacity = 0;

        var grayColor = Windows.UI.Color.FromArgb(255, 128, 128, 128);
        HomeIcon.Foreground = new SolidColorBrush(grayColor);
        CartIcon.Foreground = new SolidColorBrush(grayColor);
        SettingsIcon.Foreground = new SolidColorBrush(grayColor);

        // Activate selected tab
        switch (activeTab)
        {
            case "Home":
                HomeIndicator.Opacity = 1;
                HomeIcon.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 99, 102, 241)); // Primary color
                break;
            case "Cart":
                CartIndicator.Opacity = 1;
                CartIcon.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 99, 102, 241)); // Primary color
                break;
            case "Settings":
                SettingsIndicator.Opacity = 1;
                SettingsIcon.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 99, 102, 241)); // Primary color
                break;
        }
    }

    private void PlayPageTransition()
    {
        // Reset animation
        ContentFrame.Opacity = 0;
        var transform = ContentFrame.RenderTransform as Microsoft.UI.Xaml.Media.TranslateTransform;
        if (transform != null)
        {
            transform.X = 20;
        }

        // Start animation
        var animation = Resources["PageEnterAnimation"] as Microsoft.UI.Xaml.Media.Animation.Storyboard;
        animation?.Begin();
    }

    private void HomeTab_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CloseSidebar();
        NavigateToHome();
    }

    private void CartTab_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CloseSidebar();
        NavigateToCart();
    }

    private void SettingsTab_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CloseSidebar();
        NavigateToSettings();
    }

    private void GlobalSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Navigate to home if not already there
        if (_currentTab != "Home")
        {
            NavigateToHome();
        }
        
        // Update search in ProductListPage
        if (ContentFrame.Content is ProductListPage productListPage)
        {
            productListPage.ViewModel.SearchKeyword = GlobalSearchBox.Text;
        }
    }

    public void UpdateCartBadge(int count)
    {
        var badgeText = count > 99 ? "99+" : count.ToString();
        
        // Update bottom tab cart badge
        if (count > 0)
        {
            CartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            CartBadgeText.Text = badgeText;
        }
        else
        {
            CartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        
        // Update header cart badge
        if (count > 0)
        {
            HeaderCartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            HeaderCartBadgeText.Text = badgeText;
        }
        else
        {
            HeaderCartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    public Frame GetContentFrame()
    {
        return ContentFrame;
    }

    /// <summary>
    /// Show a toast notification with animation
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="isSuccess">True for success (green check), false for error (red X)</param>
    /// <param name="duration">Duration in milliseconds</param>
    public void ShowToast(string message, bool isSuccess = true, int duration = 2500)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                // Update toast content
                ToastMessage.Text = message;
                
                if (isSuccess)
                {
                    ToastIcon.Glyph = "\uE73E"; // Checkmark
                    ToastIcon.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 54, 116, 25)); // Green
                    ToastIconBorder.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 232, 245, 233)); // Light green
                }
                else
                {
                    ToastIcon.Glyph = "\uE711"; // X mark
                    ToastIcon.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 211, 47, 47)); // Red
                    ToastIconBorder.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 235, 238)); // Light red
                }

                // Show toast and animate in
                ToastContainer.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                AnimateToastIn();

                // Cancel previous timer if exists
                _toastTimer?.Dispose();

                // Auto hide after duration
                _toastTimer = new System.Threading.Timer(_ =>
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        AnimateToastOut();
                    });
                }, null, duration, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing toast: {ex.Message}");
            }
        });
    }

    private void AnimateToastIn()
    {
        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        
        // Slide down animation
        var slideAnimation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = -100,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(300),
            EasingFunction = new Microsoft.UI.Xaml.Media.Animation.CubicEase()
            {
                EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseOut
            }
        };
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(slideAnimation, ToastTransform);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(slideAnimation, "Y");
        
        // Fade in animation
        var fadeAnimation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(300)
        };
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fadeAnimation, ToastContainer);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
        
        storyboard.Children.Add(slideAnimation);
        storyboard.Children.Add(fadeAnimation);
        storyboard.Begin();
    }

    private void AnimateToastOut()
    {
        var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
        
        // Slide up animation
        var slideAnimation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = 0,
            To = -100,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new Microsoft.UI.Xaml.Media.Animation.CubicEase()
            {
                EasingMode = Microsoft.UI.Xaml.Media.Animation.EasingMode.EaseIn
            }
        };
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(slideAnimation, ToastTransform);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(slideAnimation, "Y");
        
        // Fade out animation
        var fadeAnimation = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation()
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(250)
        };
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fadeAnimation, ToastContainer);
        Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fadeAnimation, "Opacity");
        
        storyboard.Completed += (s, e) =>
        {
            ToastContainer.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        };
        
        storyboard.Children.Add(slideAnimation);
        storyboard.Children.Add(fadeAnimation);
        storyboard.Begin();
    }
}

