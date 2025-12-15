using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Uno_Platform.Components;

public sealed partial class Pagination : UserControl
{
    public event EventHandler<int>? PageChanged;

    #region Dependency Properties

    public static readonly DependencyProperty CurrentPageProperty =
        DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(Pagination),
            new PropertyMetadata(1, OnPaginationPropertyChanged));

    public static readonly DependencyProperty TotalPagesProperty =
        DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(Pagination),
            new PropertyMetadata(1, OnPaginationPropertyChanged));

    public static readonly DependencyProperty TotalItemsProperty =
        DependencyProperty.Register(nameof(TotalItems), typeof(int), typeof(Pagination),
            new PropertyMetadata(0, OnPaginationPropertyChanged));

    public static readonly DependencyProperty PageSizeProperty =
        DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(Pagination),
            new PropertyMetadata(5, OnPaginationPropertyChanged));

    public int CurrentPage
    {
        get => (int)GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    public int TotalPages
    {
        get => (int)GetValue(TotalPagesProperty);
        set => SetValue(TotalPagesProperty, value);
    }

    public int TotalItems
    {
        get => (int)GetValue(TotalItemsProperty);
        set => SetValue(TotalItemsProperty, value);
    }

    public int PageSize
    {
        get => (int)GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    #endregion

    public Pagination()
    {
        this.InitializeComponent();
        UpdateUI();
    }

    private static void OnPaginationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Pagination pagination)
            pagination.UpdateUI();
    }

    private void UpdateUI()
    {
        // Update page info text
        int startItem = TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        int endItem = Math.Min(CurrentPage * PageSize, TotalItems);
        
        CurrentRangeText.Text = TotalItems == 0 ? "0" : $"{startItem}-{endItem}";
        TotalItemsText.Text = TotalItems.ToString();

        // Update button states
        PrevButton.IsEnabled = CurrentPage > 1;
        NextButton.IsEnabled = CurrentPage < TotalPages;
        
        // Update button opacity for visual feedback
        PrevButton.Opacity = CurrentPage > 1 ? 1.0 : 0.5;
        NextButton.Opacity = CurrentPage < TotalPages ? 1.0 : 0.5;
    }

    private void PrevPage_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentPage > 1)
            GoToPage(CurrentPage - 1);
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentPage < TotalPages)
            GoToPage(CurrentPage + 1);
    }

    private void FirstPage_Click(object sender, RoutedEventArgs e) => GoToPage(1);
    
    private void LastPage_Click(object sender, RoutedEventArgs e) => GoToPage(TotalPages);

    private void PageJump_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && 
            int.TryParse(PageJumpTextBox.Text, out int page))
        {
            GoToPage(Math.Max(1, Math.Min(page, TotalPages)));
            PageJumpTextBox.Text = string.Empty;
        }
    }

    private void GoToPage(int pageNumber)
    {
        if (pageNumber >= 1 && pageNumber <= TotalPages && pageNumber != CurrentPage)
        {
            CurrentPage = pageNumber;
            PageChanged?.Invoke(this, pageNumber);
        }
    }
}
