using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Uno_Platform.Components;

public sealed partial class Pagination : UserControl
{
    public event EventHandler<int>? PageChanged;

    #region Dependency Properties

    public static readonly DependencyProperty CurrentPageProperty =
        DependencyProperty.Register(
            nameof(CurrentPage),
            typeof(int),
            typeof(Pagination),
            new PropertyMetadata(1, OnPaginationPropertyChanged));

    public static readonly DependencyProperty TotalPagesProperty =
        DependencyProperty.Register(
            nameof(TotalPages),
            typeof(int),
            typeof(Pagination),
            new PropertyMetadata(1, OnPaginationPropertyChanged));

    public static readonly DependencyProperty TotalItemsProperty =
        DependencyProperty.Register(
            nameof(TotalItems),
            typeof(int),
            typeof(Pagination),
            new PropertyMetadata(0, OnPaginationPropertyChanged));

    public static readonly DependencyProperty PageSizeProperty =
        DependencyProperty.Register(
            nameof(PageSize),
            typeof(int),
            typeof(Pagination),
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
        {
            pagination.UpdateUI();
        }
    }

    private void UpdateUI()
    {
        UpdatePageInfo();
        UpdatePageNumbers();
        UpdateNavigationButtons();
    }

    private void UpdatePageInfo()
    {
        int startItem = TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        int endItem = Math.Min(CurrentPage * PageSize, TotalItems);
        
        CurrentRangeText.Text = TotalItems == 0 ? "0" : $"{startItem}-{endItem}";
        TotalItemsText.Text = TotalItems.ToString();
    }

    private void UpdatePageNumbers()
    {
        PageNumbersPanel.Children.Clear();

        if (TotalPages <= 0)
        {
            // Show at least page 1 when no items
            AddPageButton(1, true);
            return;
        }

        // Calculate visible page range (show max 5 page numbers)
        int maxVisiblePages = 5;
        int startPage = Math.Max(1, CurrentPage - maxVisiblePages / 2);
        int endPage = Math.Min(TotalPages, startPage + maxVisiblePages - 1);

        // Adjust start if we're near the end
        if (endPage - startPage + 1 < maxVisiblePages)
        {
            startPage = Math.Max(1, endPage - maxVisiblePages + 1);
        }

        // Add first page + ellipsis if needed
        if (startPage > 1)
        {
            AddPageButton(1, CurrentPage == 1);
            if (startPage > 2)
            {
                AddEllipsis();
            }
        }

        // Add visible page numbers
        for (int i = startPage; i <= endPage; i++)
        {
            if (i == 1 && startPage > 1) continue; // Skip if already added
            if (i == TotalPages && endPage < TotalPages) continue; // Will add later
            
            AddPageButton(i, i == CurrentPage);
        }

        // Add ellipsis + last page if needed
        if (endPage < TotalPages)
        {
            if (endPage < TotalPages - 1)
            {
                AddEllipsis();
            }
            AddPageButton(TotalPages, CurrentPage == TotalPages);
        }
    }

    private void AddPageButton(int pageNumber, bool isActive)
    {
        var button = new Button
        {
            Content = pageNumber.ToString(),
            Tag = pageNumber,
            Style = isActive 
                ? (Style)Resources["ActivePageButtonStyle"] 
                : (Style)Resources["PaginationButtonStyle"]
        };
        
        if (!isActive)
        {
            button.Click += PageNumber_Click;
        }
        
        PageNumbersPanel.Children.Add(button);
    }

    private void AddEllipsis()
    {
        var ellipsis = new TextBlock
        {
            Text = "...",
            FontSize = 14,
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextSecondaryBrush"],
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(4, 0, 4, 0)
        };
        PageNumbersPanel.Children.Add(ellipsis);
    }

    private void UpdateNavigationButtons()
    {
        bool canGoPrev = CurrentPage > 1;
        bool canGoNext = CurrentPage < TotalPages;

        FirstPageButton.IsEnabled = canGoPrev;
        PrevButton.IsEnabled = canGoPrev;
        NextButton.IsEnabled = canGoNext;
        LastPageButton.IsEnabled = canGoNext;
    }

    #region Event Handlers

    private void PageNumber_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int pageNumber)
        {
            GoToPage(pageNumber);
        }
    }

    private void FirstPage_Click(object sender, RoutedEventArgs e)
    {
        GoToPage(1);
    }

    private void PrevPage_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentPage > 1)
        {
            GoToPage(CurrentPage - 1);
        }
    }

    private void NextPage_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentPage < TotalPages)
        {
            GoToPage(CurrentPage + 1);
        }
    }

    private void LastPage_Click(object sender, RoutedEventArgs e)
    {
        GoToPage(TotalPages);
    }

    private void PageJump_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            TryJumpToPage();
        }
    }

    private void GoToPage_Click(object sender, RoutedEventArgs e)
    {
        TryJumpToPage();
    }

    private void TryJumpToPage()
    {
        if (int.TryParse(PageJumpTextBox.Text, out int pageNumber))
        {
            pageNumber = Math.Max(1, Math.Min(pageNumber, TotalPages));
            GoToPage(pageNumber);
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

    #endregion
}

