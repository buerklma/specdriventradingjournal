using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradingJournal.Core.Interfaces
{
    /// <summary>
    /// Service interface for calculating trading analytics and statistics.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Calculates overall trading statistics.
        /// </summary>
        Task<Dictionary<string, object>> GetOverallStatisticsAsync();

        /// <summary>
        /// Calculates the win rate percentage.
        /// </summary>
        Task<decimal> GetWinRateAsync();

        /// <summary>
        /// Calculates the profit factor (gross wins / gross losses).
        /// </summary>
        Task<decimal> GetProfitFactorAsync();

        /// <summary>
        /// Calculates the average R-multiple across all trades.
        /// </summary>
        Task<decimal> GetAverageRMultipleAsync();

        /// <summary>
        /// Calculates the maximum drawdown.
        /// </summary>
        Task<decimal> GetMaxDrawdownAsync();

        /// <summary>
        /// Gets statistics grouped by symbol.
        /// </summary>
        Task<Dictionary<string, decimal>> GetStatisticsBySymbolAsync();

        /// <summary>
        /// Gets statistics grouped by setup/strategy.
        /// </summary>
        Task<Dictionary<string, decimal>> GetStatisticsBySetupAsync();

        /// <summary>
        /// Gets performance metrics for a specific setup.
        /// </summary>
        Task<Dictionary<string, object>> GetPerformanceBySetupAsync(string setup);

        /// <summary>
        /// Generates equity curve data points.
        /// </summary>
        Task<List<(DateTime Date, decimal CumulativeProfitLoss)>> GetEquityCurveAsync();

        /// <summary>
        /// Gets R-multiple distribution across buckets.
        /// </summary>
        Task<Dictionary<string, int>> GetRMultipleDistributionAsync();

        /// <summary>
        /// Gets monthly performance for a specific year.
        /// </summary>
        Task<Dictionary<string, decimal>> GetMonthlyPerformanceAsync(int year);

        /// <summary>
        /// Calculates discipline score based on trade planning adherence.
        /// </summary>
        Task<decimal> GetDisciplineScoreAsync();

        /// <summary>
        /// Gets frequency of emotional states in trades.
        /// </summary>
        Task<Dictionary<string, int>> GetEmotionFrequencyAsync();

        /// <summary>
        /// Extracts common mistakes from losing trades.
        /// </summary>
        Task<List<string>> GetCommonMistakesAsync();

        /// <summary>
        /// Gets key lessons learned from trades.
        /// </summary>
        Task<List<string>> GetKeyLessonsAsync();

        /// <summary>
        /// Correlates discipline (setup presence) with profitability.
        /// </summary>
        Task<Dictionary<string, decimal>> GetDisciplineCorrelationAsync();
    }
}
