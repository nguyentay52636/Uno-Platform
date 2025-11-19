using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Uno_Platform.Services;

namespace Uno_Platform.Views;

public sealed partial class AppShell : Page
{
    private string _currentTab = "Home";

    public AppShell()
    {
        this.InitializeComponent();
        InitializeNavigation();
    }

    private void InitializeNavigation()
    {
        // Subscribe to Cart events
        var cartService = ServiceLocator.CartService;
        cartService.CartCountChanged += (s, count) =>
        {
            DispatcherQueue.TryEnqueue(() => UpdateCartBadge(count));
        };

        // Initialize badge
        _ = InitializeCartBadgeAsync();

        // Listen to frame navigation to update header visibility
        ContentFrame.Navigated += (s, e) => UpdateHeaderVisibility();

        // Set initial page
        NavigateToHome();
    }

    private async Task InitializeCartBadgeAsync()
    {
        var count = await ServiceLocator.CartService.GetCartItemCountAsync();
        DispatcherQueue.TryEnqueue(() => UpdateCartBadge(count));
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
        UpdateHeaderVisibility();
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
        UpdateHeaderVisibility();
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
        UpdateHeaderVisibility();
        PlayPageTransition();
    }

    private void UpdateHeaderVisibility()
    {
        // Show header only on Home page (ProductListPage)
        if (ContentFrame.Content is ProductListPage)
        {
            AppBarBorder.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }
        else
        {
            AppBarBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
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
        NavigateToHome();
    }

    private void CartTab_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        NavigateToCart();
    }

    private void SettingsTab_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
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
        if (count > 0)
        {
            CartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            CartBadgeText.Text = count > 99 ? "99+" : count.ToString();
        }
        else
        {
            CartBadge.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    public Frame GetContentFrame()
    {
        return ContentFrame;
    }
}

