using Microsoft.EntityFrameworkCore;
using TradingJournal.Data;

namespace TradingJournal;

public partial class App : Application
{
    public App(TradingDbContext dbContext)
    {
        InitializeComponent();

        MainPage = new AppShell();

        // Initialize database
        InitializeDatabaseAsync(dbContext).ConfigureAwait(false);
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
