using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Calculations;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for creating a new trade with automatic R:R calculation.
    /// </summary>
    public partial class NewTradeViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private string symbol = string.Empty;

        [ObservableProperty]
        private TradeDirection direction = TradeDirection.Long;

        [ObservableProperty]
        private DateTime entryDateTime = DateTime.Now;

        [ObservableProperty]
        private decimal entryPrice;

        [ObservableProperty]
        private decimal stopLoss;

        [ObservableProperty]
        private decimal takeProfit;

        [ObservableProperty]
        private decimal positionSize;

        [ObservableProperty]
        private decimal riskPercentage = 1.0m;

        [ObservableProperty]
        private string timeframe = string.Empty;

        [ObservableProperty]
        private string setupType = string.Empty;

        [ObservableProperty]
        private string? marketStructure;

        [ObservableProperty]
        private string? notes;

        [ObservableProperty]
        private decimal plannedRR;

        public NewTradeViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "New Trade";
        }

        partial void OnEntryPriceChanged(decimal value)
        {
            CalculatePlannedRR();
        }

        partial void OnStopLossChanged(decimal value)
        {
            CalculatePlannedRR();
        }

        partial void OnTakeProfitChanged(decimal value)
        {
            CalculatePlannedRR();
        }

        partial void OnDirectionChanged(TradeDirection value)
        {
            CalculatePlannedRR();
        }

        [RelayCommand]
        private void CalculatePlannedRR()
        {
            if (EntryPrice > 0 && StopLoss > 0 && TakeProfit > 0)
            {
                // For short trades, invert the calculation
                if (Direction == TradeDirection.Short)
                {
                    PlannedRR = RiskRewardCalculator.CalculatePlannedRR(EntryPrice, StopLoss, TakeProfit) * -1;
                }
                else
                {
                    PlannedRR = RiskRewardCalculator.CalculatePlannedRR(EntryPrice, StopLoss, TakeProfit);
                }
            }
        }

        [RelayCommand]
        private async Task CreateTradeAsync()
        {
            if (IsBusy)
            {
                return;
            }

            if (!ValidateInput())
            {
                return;
            }

            try
            {
                IsBusy = true;

                var trade = new Trade
                {
                    Id = Guid.NewGuid(),
                    Symbol = Symbol,
                    Direction = Direction,
                    EntryDateTime = EntryDateTime,
                    EntryPrice = EntryPrice,
                    StopLoss = StopLoss,
                    TakeProfit = TakeProfit,
                    PositionSize = PositionSize,
                    RiskPercentage = RiskPercentage,
                    Timeframe = Timeframe,
                    SetupType = SetupType,
                    MarketStructure = MarketStructure,
                    PlannedRRRatio = PlannedRR,
                    Notes = Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _tradeService.CreateTradeAsync(trade);

                await Shell.Current.DisplayAlert("Success", "Trade created successfully", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to create trade: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Symbol))
            {
                Shell.Current.DisplayAlert("Validation Error", "Symbol is required", "OK");
                return false;
            }

            if (EntryPrice <= 0)
            {
                Shell.Current.DisplayAlert("Validation Error", "Entry price must be greater than 0", "OK");
                return false;
            }

            if (StopLoss <= 0)
            {
                Shell.Current.DisplayAlert("Validation Error", "Stop loss must be greater than 0", "OK");
                return false;
            }

            if (TakeProfit <= 0)
            {
                Shell.Current.DisplayAlert("Validation Error", "Take profit must be greater than 0", "OK");
                return false;
            }

            if (PositionSize <= 0)
            {
                Shell.Current.DisplayAlert("Validation Error", "Position size must be greater than 0", "OK");
                return false;
            }

            if (RiskPercentage < 0.1m || RiskPercentage > 100)
            {
                Shell.Current.DisplayAlert("Validation Error", "Risk percentage must be between 0.1 and 100", "OK");
                return false;
            }

            // Direction-specific validation
            if (Direction == TradeDirection.Long)
            {
                if (StopLoss >= EntryPrice)
                {
                    Shell.Current.DisplayAlert("Validation Error", "For long trades, stop loss must be below entry price", "OK");
                    return false;
                }

                if (TakeProfit <= EntryPrice)
                {
                    Shell.Current.DisplayAlert("Validation Error", "For long trades, take profit must be above entry price", "OK");
                    return false;
                }
            }
            else // Short
            {
                if (StopLoss <= EntryPrice)
                {
                    Shell.Current.DisplayAlert("Validation Error", "For short trades, stop loss must be above entry price", "OK");
                    return false;
                }

                if (TakeProfit >= EntryPrice)
                {
                    Shell.Current.DisplayAlert("Validation Error", "For short trades, take profit must be below entry price", "OK");
                    return false;
                }
            }

            return true;
        }
    }
}
