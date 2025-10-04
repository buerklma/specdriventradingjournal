using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for post-trade review and lessons learned.
    /// </summary>
    public partial class ReviewViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private Guid tradeId;

        [ObservableProperty]
        private int? qualityRating;

        [ObservableProperty]
        private string? mistakes;

        [ObservableProperty]
        private string? lessonsLearned;

        public ReviewViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Trade Review";
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
                QualityRating = trade.QualityRating;
                Mistakes = trade.Mistakes;
                LessonsLearned = trade.LessonsLearned;
            }
        }

        [RelayCommand]
        private async Task SaveReviewAsync()
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

                trade.QualityRating = QualityRating;
                trade.Mistakes = Mistakes;
                trade.LessonsLearned = LessonsLearned;
                trade.UpdatedAt = DateTime.UtcNow;

                await _tradeService.UpdateTradeAsync(trade);

                await Shell.Current.DisplayAlert("Success", "Review saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save review: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
