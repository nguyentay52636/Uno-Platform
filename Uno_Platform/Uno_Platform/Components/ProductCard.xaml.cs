using Uno_Platform.Models;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Uno_Platform.Components;

public sealed partial class ProductCard : UserControl
{
    public static readonly Microsoft.UI.Xaml.DependencyProperty ProductProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(Product),
            typeof(Product),
            typeof(ProductCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public Product Product
    {
        get => (Product)GetValue(ProductProperty);
        set => SetValue(ProductProperty, value);
    }

    public static readonly Microsoft.UI.Xaml.DependencyProperty ViewDetailsCommandProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(ViewDetailsCommand),
            typeof(ICommand),
            typeof(ProductCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public ICommand ViewDetailsCommand
    {
        get => (ICommand)GetValue(ViewDetailsCommandProperty);
        set => SetValue(ViewDetailsCommandProperty, value);
    }

    public static readonly Microsoft.UI.Xaml.DependencyProperty AddToCartCommandProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(AddToCartCommand),
            typeof(ICommand),
            typeof(ProductCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public ICommand AddToCartCommand
    {
        get => (ICommand)GetValue(AddToCartCommandProperty);
        set => SetValue(AddToCartCommandProperty, value);
    }

    public ProductCard()
    {
        this.InitializeComponent();
    }

    private void ViewDetails_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewDetailsCommand != null && Product != null)
        {
            try
            {
                ViewDetailsCommand.Execute(Product);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error executing ViewDetailsCommand: {ex.Message}");
            }
        }
    }

    private void AddToCart_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        // Prevent the event from bubbling up to the Card_Tapped handler
        e.Handled = true;
        
        // The Button will automatically execute its Command.
        // We only need to stop the event from bubbling to the parent Border.
    }

    private void Card_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        // Navigate to product detail page
        if (ViewDetailsCommand != null && Product != null)
        {
            try
            {
                ViewDetailsCommand.Execute(Product);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error executing ViewDetailsCommand from Card_Tapped: {ex.Message}");
            }
        }
    }
}
