using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class NewTradeView : ContentPage
{
    public NewTradeView(NewTradeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
