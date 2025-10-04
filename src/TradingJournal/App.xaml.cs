using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TradingJournal.Data;

namespace TradingJournal;

public partial class App : Application
{
    public App(TradingDbContext dbContext)
    {
        InitializeComponent();

        // Set up global exception handling
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        MainPage = new AppShell();

        // Initialize database
        InitializeDatabaseAsync(dbContext).ConfigureAwait(false);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            LogException(ex, "Unhandled Exception");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert(
                    "Unexpected Error",
                    "An unexpected error occurred. The application will now close.\n\nPlease check the logs for details.",
                    "OK");
                Application.Current?.Quit();
            });
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogException(e.Exception, "Unobserved Task Exception");
        e.SetObserved(); // Prevent app crash

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "An error occurred in a background task. The operation may not have completed successfully.",
                "OK");
        });
    }

    private void LogException(Exception ex, string context)
    {
        // Log to debug output
        Debug.WriteLine($"[{context}] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Debug.WriteLine($"Message: {ex.Message}");
        Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

        if (ex.InnerException != null)
        {
            Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }

        // In a production app, you would log to a file or logging service
        // For example: File.AppendAllText(logPath, logMessage);
    }

    private static async Task InitializeDatabaseAsync(TradingDbContext dbContext)
    {
        try
        {
            // Ensure database directory exists
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TradingJournal");

            if (!Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            // Apply pending migrations
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Show error to user
            await Shell.Current.DisplayAlert(
                "Database Error",
                $"Failed to initialize database: {ex.Message}\n\nThe application cannot start.",
                "OK");

            // Exit application
            Application.Current?.Quit();
        }
    }
}
