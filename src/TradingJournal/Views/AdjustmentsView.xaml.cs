using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class AdjustmentsView : ContentPage
{
    public AdjustmentsView(AdjustmentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
