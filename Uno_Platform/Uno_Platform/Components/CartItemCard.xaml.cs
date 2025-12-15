using Uno_Platform.Models;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;

namespace Uno_Platform.Components;

public sealed partial class CartItemCard : UserControl
{
    public static readonly Microsoft.UI.Xaml.DependencyProperty CartItemProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(CartItem),
            typeof(CartItem),
            typeof(CartItemCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public CartItem CartItem
    {
        get => (CartItem)GetValue(CartItemProperty);
        set => SetValue(CartItemProperty, value);
    }

    public static readonly Microsoft.UI.Xaml.DependencyProperty IncreaseQuantityCommandProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(IncreaseQuantityCommand),
            typeof(ICommand),
            typeof(CartItemCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public ICommand IncreaseQuantityCommand
    {
        get => (ICommand)GetValue(IncreaseQuantityCommandProperty);
        set => SetValue(IncreaseQuantityCommandProperty, value);
    }

    public static readonly Microsoft.UI.Xaml.DependencyProperty DecreaseQuantityCommandProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(DecreaseQuantityCommand),
            typeof(ICommand),
            typeof(CartItemCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public ICommand DecreaseQuantityCommand
    {
        get => (ICommand)GetValue(DecreaseQuantityCommandProperty);
        set => SetValue(DecreaseQuantityCommandProperty, value);
    }

    public static readonly Microsoft.UI.Xaml.DependencyProperty RemoveItemCommandProperty =
        Microsoft.UI.Xaml.DependencyProperty.Register(
            nameof(RemoveItemCommand),
            typeof(ICommand),
            typeof(CartItemCard),
            new Microsoft.UI.Xaml.PropertyMetadata(null));

    public ICommand RemoveItemCommand
    {
        get => (ICommand)GetValue(RemoveItemCommandProperty);
        set => SetValue(RemoveItemCommandProperty, value);
    }

    public CartItemCard()
    {
        this.InitializeComponent();
    }

    private void IncreaseQuantity_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (CartItem == null) return;
        
        CartItem.Quantity++;
        
        if (IncreaseQuantityCommand != null && IncreaseQuantityCommand.CanExecute(CartItem))
        {
            IncreaseQuantityCommand.Execute(CartItem);
        }
    }

    private void DecreaseQuantity_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (CartItem == null) return;
        
        if (CartItem.Quantity > 1)
        {
            CartItem.Quantity--;
            
            if (DecreaseQuantityCommand != null && DecreaseQuantityCommand.CanExecute(CartItem))
            {
                DecreaseQuantityCommand.Execute(CartItem);
            }
        }
    }

    private void Remove_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (CartItem == null) return;
        
        if (RemoveItemCommand != null && RemoveItemCommand.CanExecute(CartItem))
        {
            RemoveItemCommand.Execute(CartItem);
        }
    }
}
