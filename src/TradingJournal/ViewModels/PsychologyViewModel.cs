using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for recording trading psychology and emotions.
    /// </summary>
    public partial class PsychologyViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private Guid tradeId;

        [ObservableProperty]
        private string? emotionAtEntry;

        [ObservableProperty]
        private string? emotionDuringTrade;

        [ObservableProperty]
        private string? emotionAtExit;

        [ObservableProperty]
        private int? disciplineScore;

        [ObservableProperty]
        private string? notes;

        public PsychologyViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Psychology";
        }

        public async Task LoadTradeAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            TradeId = id;
            var trade = await _tradeService.GetTradeByIdAsync(id);

            if (trade != null)
            {
                EmotionAtEntry = trade.EmotionAtEntry;
                EmotionDuringTrade = trade.EmotionDuringTrade;
                EmotionAtExit = trade.EmotionAtExit;
                DisciplineScore = trade.DisciplineScore;
                Notes = trade.Notes;
            }
        }

        [RelayCommand]
        private async Task SavePsychologyAsync()
        {
            if (IsBusy || TradeId == Guid.Empty)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var trade = await _tradeService.GetTradeByIdAsync(TradeId);
                if (trade == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Trade not found", "OK");
                    return;
                }

                trade.EmotionAtEntry = EmotionAtEntry;
                trade.EmotionDuringTrade = EmotionDuringTrade;
                trade.EmotionAtExit = EmotionAtExit;
                trade.DisciplineScore = DisciplineScore;
                trade.Notes = Notes;
                trade.UpdatedAt = DateTime.UtcNow;

                await _tradeService.UpdateTradeAsync(trade);

                await Shell.Current.DisplayAlert("Success", "Psychology notes saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save psychology notes: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
