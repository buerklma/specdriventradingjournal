using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class PsychologyView : ContentPage
{
    public PsychologyView(PsychologyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
