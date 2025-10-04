using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class TradesListView : ContentPage
{
    public TradesListView(TradesListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TradesListViewModel viewModel)
        {
            await viewModel.LoadTradesPagedCommand.ExecuteAsync(null);
        }
    }
}
