using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for displaying and editing trade details.
    /// </summary>
    [QueryProperty(nameof(TradeId), "Id")]
    public partial class TradeDetailViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private Trade? currentTrade;

        [ObservableProperty]
        private Guid tradeId;

        public TradeDetailViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Trade Details";
        }

        partial void OnTradeIdChanged(Guid value)
        {
            if (value != Guid.Empty)
            {
                _ = LoadTradeAsync(value);
            }
        }

        [RelayCommand]
        private async Task LoadTradeAsync(Guid id)
        {
            if (IsBusy || id == Guid.Empty)
            {
                return;
            }

            try
            {
                IsBusy = true;
                CurrentTrade = await _tradeService.GetTradeByIdAsync(id);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveTradeAsync()
        {
            if (IsBusy || CurrentTrade == null)
            {
                return;
            }

            try
            {
                IsBusy = true;
                await _tradeService.UpdateTradeAsync(CurrentTrade);
                await Shell.Current.DisplayAlert("Success", "Trade saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save trade: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTradeAsync()
        {
            if (IsBusy || CurrentTrade == null)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Trade",
                "Are you sure you want to delete this trade?",
                "Yes",
                "No");

            if (!confirm)
            {
                return;
            }

            try
            {
                IsBusy = true;
                var success = await _tradeService.DeleteTradeAsync(CurrentTrade.Id);

                if (success)
                {
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete trade", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete trade: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
