using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Uno_Platform.Models;

namespace Uno_Platform.Components;

public sealed partial class CategoriesPanel : UserControl
{
    public event EventHandler<string>? CategorySelected;

    public static readonly DependencyProperty CategoriesProperty =
        DependencyProperty.Register(
            nameof(Categories),
            typeof(IEnumerable<CategoryWithCount>),
            typeof(CategoriesPanel),
            new PropertyMetadata(null, OnCategoriesChanged));

    public IEnumerable<CategoryWithCount>? Categories
    {
        get => (IEnumerable<CategoryWithCount>?)GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public CategoriesPanel()
    {
        InitializeComponent();
    }

    private static void OnCategoriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CategoriesPanel panel)
        {
            panel.CategoriesItemsControl.ItemsSource = e.NewValue as IEnumerable<CategoryWithCount>;
        }
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string categoryName)
        {
            CategorySelected?.Invoke(this, categoryName);
        }
    }
}
