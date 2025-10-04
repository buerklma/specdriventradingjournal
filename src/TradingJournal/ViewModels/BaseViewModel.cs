using CommunityToolkit.Mvvm.ComponentModel;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels providing common functionality.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title = string.Empty;
    }
}
