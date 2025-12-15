using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Uno_Platform.Models;
using Uno_Platform.Services;

namespace Uno_Platform.ViewModels;

public partial class CartViewModel : ObservableObject
{
    private readonly ICartService _cartService;

    [ObservableProperty]
    private ObservableCollection<CartItem> cartItems = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private decimal totalPrice;

    [ObservableProperty]
    private int totalItems;

    public string TotalItemsFormatted => $"{TotalItems} sản phẩm trong giỏ";
    
    public bool IsCartEmpty => CartItems.Count == 0;
    
    public bool HasCartItems => CartItems.Count > 0;

    public CartViewModel()
    {
        _cartService = ServiceLocator.CartService;
    }

    [RelayCommand]
    private async Task LoadCart()
    {
        IsLoading = true;
        try
        {
            var items = await _cartService.GetCartItemsAsync();
            CartItems.Clear();
            foreach (var item in items)
            {
                CartItems.Add(item);
            }
            UpdateTotals();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading cart: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task UpdateQuantity(CartItem? item)
    {
        if (item == null) return;

        IsLoading = true;
        try
        {
            var success = await _cartService.UpdateCartItemQuantityAsync(item.Id, item.Quantity);
            if (success)
            {
                await LoadCart();
            }
        }
        catch (Exception ex)
        {
            ToastService.Instance.ShowError("Failed to update quantity");
            System.Diagnostics.Debug.WriteLine($"Error updating quantity: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RemoveItem(CartItem? item)
    {
        if (item == null) return;

        IsLoading = true;
        try
        {
            var success = await _cartService.RemoveFromCartAsync(item.Id);
            if (success)
            {
                ToastService.Instance.ShowSuccess($"{item.ProductName} removed from cart");
                LoadCartCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            ToastService.Instance.ShowError("Failed to remove item");
            System.Diagnostics.Debug.WriteLine($"Error removing item: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ClearCart()
    {
        IsLoading = true;
        try
        {
            var success = await _cartService.ClearCartAsync();
            if (success)
            {
                ToastService.Instance.ShowSuccess("Cart cleared");
                LoadCartCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            ToastService.Instance.ShowError("Failed to clear cart");
            System.Diagnostics.Debug.WriteLine($"Error clearing cart: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Checkout()
    {
        if (CartItems.Count == 0)
        {
            ToastService.Instance.ShowError("Giỏ hàng trống");
            return;
        }

        // Navigate to checkout page
        var pageType = typeof(Uno_Platform.Views.CheckoutPage);
        ServiceLocator.NavigationService.NavigateTo(pageType);
    }

    [RelayCommand]
    private void ContinueShopping()
    {
        var pageType = typeof(Uno_Platform.Views.ProductListPage);
        ServiceLocator.NavigationService.NavigateTo(pageType);
    }

    [RelayCommand]
    private void GoBack()
    {
        ServiceLocator.NavigationService.GoBack();
    }

    private void UpdateTotals()
    {
        TotalItems = CartItems.Sum(item => item.Quantity);
        TotalPrice = CartItems.Sum(item => item.TotalPrice);
        OnPropertyChanged(nameof(TotalItemsFormatted));
        OnPropertyChanged(nameof(IsCartEmpty));
        OnPropertyChanged(nameof(HasCartItems));
    }
}
