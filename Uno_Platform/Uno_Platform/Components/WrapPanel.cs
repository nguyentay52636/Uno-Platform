using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Uno_Platform.Components;

public class WrapPanel : Panel
{
    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(WrapPanel),
            new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(WrapPanel),
            new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public double ItemWidth
    {
        get => (double)GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    public double ItemHeight
    {
        get => (double)GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WrapPanel panel)
        {
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double x = 0;
        double y = 0;
        double rowHeight = 0;
        double maxWidth = 0;

        foreach (UIElement child in Children)
        {
            var childSize = new Size(
                double.IsNaN(ItemWidth) ? availableSize.Width : ItemWidth,
                double.IsNaN(ItemHeight) ? availableSize.Height : ItemHeight);

            child.Measure(childSize);

            var desiredWidth = double.IsNaN(ItemWidth) ? child.DesiredSize.Width : ItemWidth;
            var desiredHeight = double.IsNaN(ItemHeight) ? child.DesiredSize.Height : ItemHeight;

            if (x + desiredWidth > availableSize.Width && x > 0)
            {
                x = 0;
                y += rowHeight;
                rowHeight = 0;
            }

            x += desiredWidth;
            rowHeight = Math.Max(rowHeight, desiredHeight);
            maxWidth = Math.Max(maxWidth, x);
        }

        return new Size(maxWidth, y + rowHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double x = 0;
        double y = 0;
        double rowHeight = 0;

        foreach (UIElement child in Children)
        {
            var desiredWidth = double.IsNaN(ItemWidth) ? child.DesiredSize.Width : ItemWidth;
            var desiredHeight = double.IsNaN(ItemHeight) ? child.DesiredSize.Height : ItemHeight;

            if (x + desiredWidth > finalSize.Width && x > 0)
            {
                x = 0;
                y += rowHeight;
                rowHeight = 0;
            }

            child.Arrange(new Rect(x, y, desiredWidth, desiredHeight));

            x += desiredWidth;
            rowHeight = Math.Max(rowHeight, desiredHeight);
        }

        return finalSize;
    }
}
