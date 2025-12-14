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

    private void Pagination_PageChanged(object sender, int pageNumber)
    {
        ViewModel.GoToPageCommand.Execute(pageNumber);
        UpdateEmptyState();
    }
}
