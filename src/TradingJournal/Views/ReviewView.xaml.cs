using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class ReviewView : ContentPage
{
    public ReviewView(ReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
