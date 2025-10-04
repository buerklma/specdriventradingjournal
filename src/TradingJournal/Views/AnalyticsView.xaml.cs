using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class AnalyticsView : ContentPage
{
    public AnalyticsView(AnalyticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
