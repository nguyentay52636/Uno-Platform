using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System.Collections.ObjectModel;
using Uno_Platform.Models;
using Uno_Platform.Services;

namespace Uno_Platform.ViewModels;

public partial class ProductListViewModel : ObservableObject
{
    private readonly ProductService _productService;
    private readonly CartService _cartService;
    private System.Threading.Timer? _searchDebounceTimer;

    ~ProductListViewModel()
    {
        // Cleanup timer on finalization
        _searchDebounceTimer?.Dispose();
    }

    [ObservableProperty]
    private List<Product> products = new();

    [ObservableProperty]
    private ObservableCollection<Product> filteredProducts = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string searchKeyword = string.Empty;

    [ObservableProperty]
    private decimal minPrice;

    [ObservableProperty]
    private decimal maxPrice = 10000;

    [ObservableProperty]
    private decimal currentMinPrice;

    [ObservableProperty]
    private decimal currentMaxPrice = 10000;

    public string CurrentMinPriceFormatted => CurrentMinPrice.ToString("F0");
    
    public string CurrentMaxPriceFormatted => CurrentMaxPrice.ToString("F0");

    [ObservableProperty]
    private string selectedCategory = "All";

    [ObservableProperty]
    private List<string> categories = new();

    [ObservableProperty]
    private List<CategoryWithCount> categoriesWithCount = new();

    [ObservableProperty]
    private int cartItemCount;

    public ProductListViewModel()
    {
        _productService = ServiceLocator.ProductService;
        _cartService = ServiceLocator.CartService;
        LoadData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            Products = await _productService.GetAllProductsAsync();
            Categories = await _productService.GetAllCategoriesAsync();
            Categories.Insert(0, "All");
            
            // Calculate product counts for each category
            CategoriesWithCount = Categories.Select(category =>
            {
                int count = category == "All" 
                    ? Products.Count 
                    : Products.Count(p => p.Category == category);
                return new CategoryWithCount { Name = category, Count = count };
            }).ToList();
            
            if (Products.Any())
            {
                MinPrice = Products.Min(p => p.Price);
                MaxPrice = Products.Max(p => p.Price);
                
                // Set initial price range to show all products
                CurrentMinPrice = MinPrice;
                CurrentMaxPrice = MaxPrice;
                
                // Notify property changes
                OnPropertyChanged(nameof(CurrentMinPriceFormatted));
                OnPropertyChanged(nameof(CurrentMaxPriceFormatted));
            }
            
            ApplyFilters();
            await UpdateCartCount();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void LoadData()
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void Search()
    {
        // Debounce search - increased to 500ms for better performance
        _searchDebounceTimer?.Dispose();
        _searchDebounceTimer = new System.Threading.Timer(_ =>
        {
            DispatcherQueue.GetForCurrentThread()?.TryEnqueue(() =>
            {
                ApplyFilters();
            });
        }, null, 500, Timeout.Infinite);
    }

    partial void OnSearchKeywordChanged(string value)
    {
        Search();
    }

    partial void OnCurrentMinPriceChanged(decimal value)
    {
        OnPropertyChanged(nameof(CurrentMinPriceFormatted));
        ApplyFilters();
    }

    partial void OnCurrentMaxPriceChanged(decimal value)
    {
        OnPropertyChanged(nameof(CurrentMaxPriceFormatted));
        ApplyFilters();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilters();
        // Update category counts after filtering
        UpdateCategoryCounts();
    }

    private void UpdateCategoryCounts()
    {
        CategoriesWithCount = Categories.Select(category =>
        {
            int count = category == "All" 
                ? Products.Count 
                : Products.Count(p => p.Category == category);
            return new CategoryWithCount { Name = category, Count = count };
        }).ToList();
    }

    private void ApplyFilters()
    {
        try
        {
            var filtered = Products.AsEnumerable();

            // Category filter
            if (SelectedCategory != "All")
            {
                filtered = filtered.Where(p => p.Category == SelectedCategory);
            }

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var keyword = SearchKeyword.ToLowerInvariant();
                filtered = filtered.Where(p =>
                    p.Name.ToLowerInvariant().Contains(keyword) ||
                    p.Category.ToLowerInvariant().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLowerInvariant().Contains(keyword))
                );
            }

            // Optimize collection update - only update if different
            var filteredList = filtered.ToList();
            
            // Check if we need to update
            if (FilteredProducts.Count != filteredList.Count || 
                !FilteredProducts.SequenceEqual(filteredList))
            {
                FilteredProducts.Clear();
                foreach (var product in filteredList)
                {
                    FilteredProducts.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
            FilteredProducts.Clear();
        }
    }

    [RelayCommand]
    private async Task AddToCart(Product? product)
    {
        if (product == null)
        {
            System.Diagnostics.Debug.WriteLine("AddToCart: product is null");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"AddToCart called for product: {product.Name}, Id: {product.Id}");
        
        try
        {
            var success = await _cartService.AddToCartAsync(product.Id);
            System.Diagnostics.Debug.WriteLine($"AddToCart result: {success}");
            if (success)
            {
                await UpdateCartCount();
                ToastService.Instance.ShowSuccess($"{product.Name} added to cart!");
            }
            else
            {
                ToastService.Instance.ShowError("Failed to add to cart");
            }
        }
        catch (Exception ex)
        {
            ToastService.Instance.ShowError("Failed to add to cart");
            System.Diagnostics.Debug.WriteLine($"Error adding to cart: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    [RelayCommand]
    private void NavigateToProductDetail(Product? product)
    {
        if (product == null)
        {
            System.Diagnostics.Debug.WriteLine("NavigateToProductDetail: product is null");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"NavigateToProductDetail called for product: {product.Name}, Id: {product.Id}");
        
        try
        {
            var pageType = typeof(Uno_Platform.Views.ProductDetailPage);
            var success = ServiceLocator.NavigationService.NavigateTo(pageType, product);
            System.Diagnostics.Debug.WriteLine($"Navigation result: {success}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to product detail: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    [RelayCommand]
    private void NavigateToCart()
    {
        var pageType = typeof(Uno_Platform.Views.CartPage);
        ServiceLocator.NavigationService.NavigateTo(pageType);
    }

    [RelayCommand]
    public async Task Refresh()
    {
        await LoadDataAsync();
    }

    private async Task UpdateCartCount()
    {
        try
        {
            CartItemCount = await _cartService.GetCartItemCountAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating cart count: {ex.Message}");
        }
    }
}
