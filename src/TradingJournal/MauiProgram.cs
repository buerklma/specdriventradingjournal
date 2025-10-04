using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TradingJournal.Core.Interfaces;
using TradingJournal.Core.Services;
using TradingJournal.Data;
using TradingJournal.ViewModels;
using TradingJournal.Views;

namespace TradingJournal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Database Configuration
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TradingJournal",
            "tradingjournal.db");

        builder.Services.AddDbContext<TradingDbContext>(
            options => options.UseSqlite($"Data Source={dbPath}"),
            ServiceLifetime.Scoped);

        // Register Services
        builder.Services.AddScoped<ITradeService, TradeService>();
        builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
        builder.Services.AddScoped<IExportService, ExportService>();

        // Register ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TradesListViewModel>();
        builder.Services.AddTransient<TradeDetailViewModel>();
        builder.Services.AddTransient<NewTradeViewModel>();
        builder.Services.AddTransient<PsychologyViewModel>();
        builder.Services.AddTransient<ReviewViewModel>();
        builder.Services.AddTransient<AnalyticsViewModel>();
        builder.Services.AddTransient<ExportViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<AttachmentsViewModel>();
        builder.Services.AddTransient<AdjustmentsViewModel>();

        // Register Views
        builder.Services.AddTransient<DashboardView>();
        builder.Services.AddTransient<TradesListView>();
        builder.Services.AddTransient<TradeDetailView>();
        builder.Services.AddTransient<NewTradeView>();
        builder.Services.AddTransient<PsychologyView>();
        builder.Services.AddTransient<ReviewView>();
        builder.Services.AddTransient<AnalyticsView>();
        builder.Services.AddTransient<ExportView>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<AttachmentsView>();
        builder.Services.AddTransient<AdjustmentsView>();

        return builder.Build();
    }
}
