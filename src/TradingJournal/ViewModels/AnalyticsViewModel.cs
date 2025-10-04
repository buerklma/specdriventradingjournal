using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for analytics charts and performance metrics.
    /// </summary>
    public partial class AnalyticsViewModel : BaseViewModel
    {
        private readonly IAnalyticsService _analyticsService;

        [ObservableProperty]
        private List<object>? equityCurveData;

        [ObservableProperty]
        private List<object>? rrDistributionData;

        [ObservableProperty]
        private List<object>? performanceBySetupData;

        [ObservableProperty]
        private decimal winRate;

        [ObservableProperty]
        private decimal profitFactor;

        [ObservableProperty]
        private decimal averageRMultiple;

        [ObservableProperty]
        private decimal maxDrawdown;

        public AnalyticsViewModel(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
            Title = "Analytics";
        }

        [RelayCommand]
        private async Task LoadAnalyticsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // Load statistics
                WinRate = await _analyticsService.GetWinRateAsync();
                ProfitFactor = await _analyticsService.GetProfitFactorAsync();
                AverageRMultiple = await _analyticsService.GetAverageRMultipleAsync();
                MaxDrawdown = await _analyticsService.GetMaxDrawdownAsync();

                // Load chart data
                var equityCurve = await _analyticsService.GetEquityCurveAsync();
                EquityCurveData = equityCurve.Cast<object>().ToList();

                var rrDistribution = await _analyticsService.GetRMultipleDistributionAsync();
                RrDistributionData = rrDistribution.Cast<object>().ToList();

                var performanceBySetup = await _analyticsService.GetStatisticsBySetupAsync();
                PerformanceBySetupData = performanceBySetup.Cast<object>().ToList();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
