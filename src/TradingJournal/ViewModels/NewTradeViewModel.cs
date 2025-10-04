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

        [ObservableProperty]
        private string? symbolError;

        [ObservableProperty]
        private string? entryPriceError;

        [ObservableProperty]
        private string? stopLossError;

        [ObservableProperty]
        private string? takeProfitError;

        [ObservableProperty]
        private string? positionSizeError;

        [ObservableProperty]
        private string? riskPercentageError;

        [ObservableProperty]
        private bool isValid;

        public NewTradeViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "New Trade";
        }

        partial void OnEntryPriceChanged(decimal value)
        {
            CalculatePlannedRR();
            ValidateInput();
        }

        partial void OnStopLossChanged(decimal value)
        {
            CalculatePlannedRR();
            ValidateInput();
        }

        partial void OnTakeProfitChanged(decimal value)
        {
            CalculatePlannedRR();
            ValidateInput();
        }

        partial void OnDirectionChanged(TradeDirection value)
        {
            CalculatePlannedRR();
            ValidateInput();
        }

        partial void OnSymbolChanged(string value)
        {
            ValidateInput();
        }

        partial void OnPositionSizeChanged(decimal value)
        {
            ValidateInput();
        }

        partial void OnRiskPercentageChanged(decimal value)
        {
            ValidateInput();
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
            // Clear previous errors
            SymbolError = null;
            EntryPriceError = null;
            StopLossError = null;
            TakeProfitError = null;
            PositionSizeError = null;
            RiskPercentageError = null;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(Symbol))
            {
                SymbolError = "Symbol is required";
                isValid = false;
            }

            if (EntryPrice <= 0)
            {
                EntryPriceError = "Entry price must be greater than 0";
                isValid = false;
            }

            if (StopLoss <= 0)
            {
                StopLossError = "Stop loss must be greater than 0";
                isValid = false;
            }

            if (TakeProfit <= 0)
            {
                TakeProfitError = "Take profit must be greater than 0";
                isValid = false;
            }

            if (PositionSize <= 0)
            {
                PositionSizeError = "Position size must be greater than 0";
                isValid = false;
            }

            if (RiskPercentage < 0.1m || RiskPercentage > 100)
            {
                RiskPercentageError = "Risk percentage must be between 0.1 and 100";
                isValid = false;
            }

            // Direction-specific validation
            if (EntryPrice > 0 && StopLoss > 0 && TakeProfit > 0)
            {
                if (Direction == TradeDirection.Long)
                {
                    if (StopLoss >= EntryPrice)
                    {
                        StopLossError = "For long trades, stop loss must be below entry price";
                        isValid = false;
                    }

                    if (TakeProfit <= EntryPrice)
                    {
                        TakeProfitError = "For long trades, take profit must be above entry price";
                        isValid = false;
                    }
                }
                else // Short
                {
                    if (StopLoss <= EntryPrice)
                    {
                        StopLossError = "For short trades, stop loss must be above entry price";
                        isValid = false;
                    }

                    if (TakeProfit >= EntryPrice)
                    {
                        TakeProfitError = "For short trades, take profit must be below entry price";
                        isValid = false;
                    }
                }
            }

            IsValid = isValid;
            return isValid;
        }
    }
}
