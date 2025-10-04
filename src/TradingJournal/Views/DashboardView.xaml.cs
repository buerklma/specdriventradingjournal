using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class DashboardView : ContentPage
{
    public DashboardView(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DashboardViewModel viewModel)
        {
            await viewModel.LoadStatisticsCommand.ExecuteAsync(null);
        }
    }
}
