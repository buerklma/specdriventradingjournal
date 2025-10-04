using TradingJournal.ViewModels;

namespace TradingJournal.Views;

public partial class AttachmentsView : ContentPage
{
    public AttachmentsView(AttachmentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
