using Uno_Platform.ViewModels;
using Uno_Platform.Models;
using Uno_Platform.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;

namespace Uno_Platform.Views;

public sealed partial class ProductListPage : Page
{
    public ProductListViewModel ViewModel { get; }

    public ProductListPage()
    {
        this.InitializeComponent();
        ViewModel = new ProductListViewModel();
        this.DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        PageEnterAnimation.Begin();
        UpdateEmptyState();
        _ = ViewModel.RefreshCommand.ExecuteAsync(null);
        
        // Update category button styles after data loads
        this.Loaded += (s, args) => UpdateCategoryButtonStyles();
    }

    private async void LoadSampleData_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== Manual Load Sample Data clicked ===");
            
            var button = sender as Button;
            if (button != null)
            {
                button.IsEnabled = false;
                button.Content = "⏳ Loading...";
            }
            
            // Clear existing products first to force re-seed
            var products = await ServiceLocator.ProductRepository.GetAllProductsAsync();
            System.Diagnostics.Debug.WriteLine($"Current products count: {products.Count}");
            
            // Force seed data
            await ServiceLocator.DataSeedService.SeedDataAsync();
            
            // Refresh the list
            await ViewModel.RefreshCommand.ExecuteAsync(null);
            
            if (button != null)
            {
                button.Content = "✅ Data Loaded!";
                await Task.Delay(2000);
                button.Content = "🔄 Load Sample Data";
                button.IsEnabled = true;
            }
            
            ToastService.Instance.ShowSuccess($"Loaded {ViewModel.FilteredProducts.Count} products!");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error loading sample data: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            ToastService.Instance.ShowError($"Error: {ex.Message}");
            
            var button = sender as Button;
            if (button != null)
            {
                button.Content = "❌ Error - Try Again";
                button.IsEnabled = true;
            }
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        if (EmptyStateBorder != null && ViewModel != null)
        {
            EmptyStateBorder.Visibility = ViewModel.FilteredProducts.Count == 0
                ? Microsoft.UI.Xaml.Visibility.Visible
                : Microsoft.UI.Xaml.Visibility.Collapsed;
        }
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string category)
        {
             ViewModel.SelectedCategory = category;
             UpdateEmptyState();
             UpdateCategoryButtonStyles();
        }
    }

    private void UpdateCategoryButtonStyles()
    {
        // Use Dispatcher to update after UI is rendered
        _ = DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() =>
        {
            if (CategoriesItemsControl != null)
            {
                UpdateButtonStylesRecursive(CategoriesItemsControl);
            }
            
            // Force ItemsControl to refresh
            if (ProductsItemsControl != null)
            {
                ProductsItemsControl.UpdateLayout();
            }
        });
    }

    private void UpdateButtonStylesRecursive(Microsoft.UI.Xaml.DependencyObject parent)
    {
        int childrenCount = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
            
            if (child is Microsoft.UI.Xaml.Controls.Button button && button.Tag is string categoryName)
            {
                if (categoryName == ViewModel.SelectedCategory)
                {
                    button.Background = Application.Current.Resources["PrimaryBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    button.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White);
                    button.BorderThickness = new Microsoft.UI.Xaml.Thickness(3);
                }
                else
                {
                    button.Background = Application.Current.Resources["AcrylicBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    button.Foreground = Application.Current.Resources["TextPrimaryBrush"] as Microsoft.UI.Xaml.Media.Brush;
                    button.BorderThickness = new Microsoft.UI.Xaml.Thickness(2);
                }
            }
            
            UpdateButtonStylesRecursive(child);
        }
    }
}
