using Uno_Platform.ViewModels;
using Uno_Platform.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Uno_Platform.Views;

public sealed partial class ProductEditPage : Page
{
    public ProductEditViewModel ViewModel { get; }

    public ProductEditPage()
    {
        this.InitializeComponent();
        ViewModel = new ProductEditViewModel();
        this.DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        // Start Enter Animation
        var anim = Resources["PageEnterAnimation"] as Microsoft.UI.Xaml.Media.Animation.Storyboard;
        anim?.Begin();

        if (e.Parameter is Product product)
        {
            _ = ViewModel.LoadProductAsync(product);
        }
        else
        {
            _ = ViewModel.LoadProductAsync(null); // New product mode
        }
    }


    private void PriceTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (sender == null) return;

        var text = sender.Text;
        // Allow only digits and one decimal point
        var newText = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());

        // Handle multiple dots - keep only the first one
        var firstDotIndex = newText.IndexOf('.');
        if (firstDotIndex != -1)
        {
            var beforeDot = newText.Substring(0, firstDotIndex + 1);
            var afterDot = newText.Substring(firstDotIndex + 1).Replace(".", "");
            newText = beforeDot + afterDot;
        }

        // Update text if it contains invalid characters
        if (text != newText)
        {
            var pos = sender.SelectionStart;
            // Adjust cursor position if we removed characters before it
            var diff = text.Length - newText.Length;
            
            sender.Text = newText;
            sender.SelectionStart = Math.Max(0, Math.Min(pos - diff, newText.Length));
        }

        if (decimal.TryParse(newText, out decimal price))
        {
            ViewModel.Price = price;
        }
        else if (string.IsNullOrEmpty(newText))
        {
             ViewModel.Price = 0;
        }
    }
}

