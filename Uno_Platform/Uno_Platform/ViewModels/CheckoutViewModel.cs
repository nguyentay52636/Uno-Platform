using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Uno_Platform.Models;
using Uno_Platform.Services;

namespace Uno_Platform.ViewModels;

public partial class CheckoutViewModel : ObservableObject
{
    private readonly ICartService _cartService;
    private readonly ApiService _apiService;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string customerName = string.Empty;

    [ObservableProperty]
    private string customerAddress = string.Empty;

    [ObservableProperty]
    private string customerPhone = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private decimal totalPrice;

    [ObservableProperty]
    private List<CartItem> cartItems = new();

    public string TotalPriceFormatted => $"{TotalPrice:N0} đ";

    public CheckoutViewModel()
    {
        _cartService = ServiceLocator.CartService;
        _apiService = ServiceLocator.ApiService;
    }

    [RelayCommand]
    private async Task LoadCartData()
    {
        IsLoading = true;
        try
        {
            var items = await _cartService.GetCartItemsAsync();
            CartItems = items;
            TotalPrice = items.Sum(item => item.TotalPrice);
            OnPropertyChanged(nameof(TotalPriceFormatted));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading cart data: {ex.Message}");
            ToastService.Instance.ShowError("Failed to load cart data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SubmitOrder()
    {
        ErrorMessage = string.Empty;

        // Validate input
        if (string.IsNullOrWhiteSpace(CustomerName))
        {
            ToastService.Instance.ShowError("Vui lòng nhập tên khách hàng");
            return;
        }

        if (string.IsNullOrWhiteSpace(CustomerAddress))
        {
            ToastService.Instance.ShowError("Vui lòng nhập địa chỉ giao hàng");
            return;
        }

        if (string.IsNullOrWhiteSpace(CustomerPhone))
        {
            ToastService.Instance.ShowError("Vui lòng nhập số điện thoại");
            return;
        }

        // Validate phone number: starts with 0, 10-11 digits
        var phoneDigits = new string(CustomerPhone.Where(char.IsDigit).ToArray());
        if (!phoneDigits.StartsWith("0"))
        {
            ToastService.Instance.ShowError("Số điện thoại phải bắt đầu bằng số 0");
            return;
        }
        if (phoneDigits.Length < 10 || phoneDigits.Length > 11)
        {
            ToastService.Instance.ShowError("Số điện thoại phải có 10-11 chữ số");
            return;
        }

        if (CartItems == null || !CartItems.Any())
        {
            ToastService.Instance.ShowError("Giỏ hàng trống");
            return;
        }

        IsLoading = true;
        try
        {
            // Tạo OrderModel từ giỏ hàng
            var order = new OrderModel
            {
                CustomerName = CustomerName,
                CustomerAddress = CustomerAddress,
                CustomerPhone = CustomerPhone,
                TotalPrice = TotalPrice,
                Items = CartItems.Select(item => new OrderItemModel
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            // Gọi API để đặt hàng
            var success = await _apiService.SubmitOrderAsync(order);

            if (success)
            {
                // Xóa giỏ hàng sau khi đặt hàng thành công
                await _cartService.ClearCartAsync();
                
                ToastService.Instance.ShowSuccess("Đặt hàng thành công!");
                
                // Đợi một chút để user thấy thông báo
                await Task.Delay(1000);
                
                // Chuyển đến trang thông báo thành công
                var pageType = typeof(Uno_Platform.Views.OrderSuccessPage);
                ServiceLocator.NavigationService.NavigateTo(pageType);
            }
            else
            {
                ErrorMessage = "Đặt hàng thất bại. Vui lòng thử lại.";
                ToastService.Instance.ShowError("Đặt hàng thất bại");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Có lỗi xảy ra khi đặt hàng";
            ToastService.Instance.ShowError("Có lỗi xảy ra khi đặt hàng");
            System.Diagnostics.Debug.WriteLine($"Error submitting order: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        ServiceLocator.NavigationService.GoBack();
    }
}

