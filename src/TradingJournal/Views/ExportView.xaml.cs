using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class ExportView : ContentPage
{
    public ExportView(ExportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
