using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for the dashboard displaying overall trading statistics.
    /// </summary>
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private int totalTrades;

        [ObservableProperty]
        private decimal winRate;

        [ObservableProperty]
        private decimal totalProfitLoss;

        [ObservableProperty]
        private decimal maxDrawdown;

        [ObservableProperty]
        private decimal profitFactor;

        [ObservableProperty]
        private decimal averageRMultiple;

        public DashboardViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            Title = "Dashboard";
        }

        [RelayCommand]
        private async Task LoadStatisticsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var statistics = await _analyticsService.GetOverallStatisticsAsync();

                TotalTrades = statistics.ContainsKey("TotalTrades") ? System.Convert.ToInt32(statistics["TotalTrades"]) : 0;
                TotalProfitLoss = statistics.ContainsKey("TotalProfitLoss") ? System.Convert.ToDecimal(statistics["TotalProfitLoss"]) : 0;

                WinRate = await _analyticsService.GetWinRateAsync();
                ProfitFactor = await _analyticsService.GetProfitFactorAsync();
                AverageRMultiple = await _analyticsService.GetAverageRMultipleAsync();
                MaxDrawdown = await _analyticsService.GetMaxDrawdownAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
