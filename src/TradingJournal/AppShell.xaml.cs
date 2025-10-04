using TradingJournal.Views;

namespace TradingJournal;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(TradeDetailView), typeof(TradeDetailView));
        Routing.RegisterRoute(nameof(NewTradeView), typeof(NewTradeView));
        Routing.RegisterRoute(nameof(PsychologyView), typeof(PsychologyView));
        Routing.RegisterRoute(nameof(ReviewView), typeof(ReviewView));
        Routing.RegisterRoute(nameof(AttachmentsView), typeof(AttachmentsView));
        Routing.RegisterRoute(nameof(AdjustmentsView), typeof(AdjustmentsView));
    }
}
