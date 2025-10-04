using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class TradeDetailView : ContentPage
{
    public TradeDetailView(TradeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
