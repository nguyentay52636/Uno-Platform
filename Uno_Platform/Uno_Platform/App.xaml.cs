using System;
using Microsoft.Extensions.Logging;
using Uno.Resizetizer;
using Uno_Platform.Services;
using Uno_Platform.Database;

namespace Uno_Platform;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        
#if __ANDROID__ || __IOS__ || __MACOS__
        // Initialize SQLite for mobile/desktop platforms using bundle_green
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#endif
    }

    protected Window? MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();
#if DEBUG
        MainWindow.UseStudio();
#endif

        // Initialize database and seed sample data
        InitializeDatabase();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content == null)
        {
            // Create AppShell as the main navigation container
            var appShell = new Views.AppShell();
            MainWindow.Content = appShell;
            
            // Initialize NavigationService with the AppShell's internal frame
            ServiceLocator.NavigationService.Initialize(appShell.GetContentFrame());
        }

        MainWindow.SetWindowIcon();
        // Ensure the current window is active
        MainWindow.Activate();
    }

    private void InitializeDatabase()
    {
        System.Diagnostics.Debug.WriteLine("=== App: InitializeDatabase called ===");
        // Initialize database asynchronously without blocking UI
        _ = InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== App: Starting InitializeDatabaseAsync ===");
            
            // Force initialize database connection first
#if __ANDROID__ || __IOS__ || __MACOS__
            var dbContext = ServiceLocator.DbContext;
            System.Diagnostics.Debug.WriteLine($"Database connection initialized: {dbContext != null}");
#endif
            
            await ServiceLocator.DataSeedService.SeedDataAsync();
            System.Diagnostics.Debug.WriteLine("=== App: Database initialization completed ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"=== App: CRITICAL ERROR initializing database ===");
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Show error to user on Android
#if __ANDROID__
            if (MainWindow?.DispatcherQueue != null)
            {
                MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                    {
                        Title = "Database Error",
                        Content = $"Failed to initialize database: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = MainWindow.Content?.XamlRoot
                    };
                    await dialog.ShowAsync();
                });
            }
#endif
        }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on the web or
        // desktop targets, you can use URL or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());

            // Log to the Visual Studio Debug console
            builder.AddConsole();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // DevServer and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
