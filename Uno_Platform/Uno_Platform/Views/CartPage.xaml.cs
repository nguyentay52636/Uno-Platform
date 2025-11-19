using Uno_Platform.ViewModels;
using Uno_Platform.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Uno_Platform.Views;

public sealed partial class CartPage : Page
{
    public CartViewModel ViewModel { get; }

    public CartPage()
    {
        this.InitializeComponent();
        ViewModel = new CartViewModel();
        this.DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.LoadCartCommand.Execute(null);
        
        // Start Enter Animation
        var anim = Resources["PageEnterAnimation"] as Microsoft.UI.Xaml.Media.Animation.Storyboard;
        anim?.Begin();

        UpdateEmptyState();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        // Empty state visibility is handled by binding in XAML
    }
}
