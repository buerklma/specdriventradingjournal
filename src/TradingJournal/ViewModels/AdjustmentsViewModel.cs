using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for managing trade management adjustments (stop-loss/take-profit changes).
    /// </summary>
    public partial class AdjustmentsViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private Guid tradeId;

        [ObservableProperty]
        private ObservableCollection<ManagementAdjustment> adjustments = new();

        [ObservableProperty]
        private AdjustmentType newAdjustmentType = AdjustmentType.StopLoss;

        [ObservableProperty]
        private DateTime newAdjustmentDateTime = DateTime.Now;

        [ObservableProperty]
        private decimal? newPreviousValue;

        [ObservableProperty]
        private decimal newValue;

        [ObservableProperty]
        private string? newReason;

        public AdjustmentsViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Adjustments";
        }

        public async Task LoadAdjustmentsAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            TradeId = id;

            try
            {
                var adjustmentList = await _tradeService.GetAdjustmentsForTradeAsync(id);

                Adjustments.Clear();
                foreach (var adjustment in adjustmentList)
                {
                    Adjustments.Add(adjustment);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load adjustments: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task AddAdjustmentAsync()
        {
            if (IsBusy || TradeId == Guid.Empty)
            {
                return;
            }

            if (NewValue <= 0)
            {
                await Shell.Current.DisplayAlert("Validation Error", "New value must be greater than 0", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var adjustment = new ManagementAdjustment
                {
                    Id = Guid.NewGuid(),
                    TradeId = TradeId,
                    AdjustmentType = NewAdjustmentType,
                    AdjustmentDateTime = NewAdjustmentDateTime,
                    PreviousValue = NewPreviousValue,
                    NewValue = NewValue,
                    Reason = NewReason
                };

                await _tradeService.AddAdjustmentAsync(TradeId, adjustment);

                // Reset form
                NewAdjustmentType = AdjustmentType.StopLoss;
                NewAdjustmentDateTime = DateTime.Now;
                NewPreviousValue = null;
                NewValue = 0;
                NewReason = null;

                await LoadAdjustmentsAsync(TradeId);
                await Shell.Current.DisplayAlert("Success", "Adjustment added successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to add adjustment: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
