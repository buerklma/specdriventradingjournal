using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Services
{
    /// <summary>
    /// Service for calculating trading analytics, statistics, and performance metrics.
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly TradingDbContext _context;

        public AnalyticsService(TradingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Dictionary<string, object>> GetOverallStatisticsAsync()
        {
            var trades = await _context.Trades.ToListAsync();

            var winningTrades = trades.Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value > 0).ToList();
            var losingTrades = trades.Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value < 0).ToList();

            var totalTrades = trades.Count;
            var totalProfitLoss = trades.Sum(t => t.ProfitLossCurrency ?? 0);
            var avgProfitLoss = totalTrades > 0 ? totalProfitLoss / totalTrades : 0;

            return new Dictionary<string, object>
            {
                ["TotalTrades"] = totalTrades,
                ["WinningTrades"] = winningTrades.Count,
                ["LosingTrades"] = losingTrades.Count,
                ["TotalProfitLoss"] = totalProfitLoss,
                ["AverageProfitLoss"] = avgProfitLoss,
                ["LargestWin"] = winningTrades.Any() ? winningTrades.Max(t => t.ProfitLossCurrency ?? 0) : 0,
                ["LargestLoss"] = losingTrades.Any() ? losingTrades.Min(t => t.ProfitLossCurrency ?? 0) : 0
            };
        }

        public async Task<decimal> GetWinRateAsync()
        {
            var trades = await _context.Trades.ToListAsync();
            if (!trades.Any())
            {
                return 0;
            }

            var winningTrades = trades.Count(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value > 0);
            return (decimal)winningTrades / trades.Count * 100;
        }

        public async Task<decimal> GetProfitFactorAsync()
        {
            var trades = await _context.Trades.ToListAsync();

            var totalWins = trades.Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value > 0).Sum(t => t.ProfitLossCurrency!.Value);
            var totalLosses = Math.Abs(trades.Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value < 0).Sum(t => t.ProfitLossCurrency!.Value));

            if (totalLosses == 0)
            {
                return totalWins > 0 ? decimal.MaxValue : 0;
            }

            return totalWins / totalLosses;
        }

        public async Task<decimal> GetAverageRMultipleAsync()
        {
            var trades = await _context.Trades.Where(t => t.RealizedRRRatio.HasValue).ToListAsync();
            if (!trades.Any())
            {
                return 0;
            }

            return trades.Average(t => t.RealizedRRRatio!.Value);
        }

        public async Task<decimal> GetMaxDrawdownAsync()
        {
            var trades = await _context.Trades
                .OrderBy(t => t.EntryDateTime)
                .ToListAsync();

            if (!trades.Any())
            {
                return 0;
            }

            decimal runningTotal = 0;
            decimal peak = 0;
            decimal maxDrawdown = 0;

            foreach (var trade in trades)
            {
                runningTotal += trade.ProfitLossCurrency ?? 0;
                if (runningTotal > peak)
                {
                    peak = runningTotal;
                }

                var drawdown = peak - runningTotal;
                if (drawdown > maxDrawdown)
                {
                    maxDrawdown = drawdown;
                }
            }

            return maxDrawdown;
        }

        public async Task<Dictionary<string, decimal>> GetStatisticsBySymbolAsync()
        {
            var trades = await _context.Trades.ToListAsync();

            return trades
                .GroupBy(t => t.Symbol)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => t.ProfitLossCurrency ?? 0)
                );
        }

        public async Task<Dictionary<string, decimal>> GetStatisticsBySetupAsync()
        {
            var trades = await _context.Trades.ToListAsync();

            return trades
                .Where(t => !string.IsNullOrEmpty(t.SetupType))
                .GroupBy(t => t.SetupType!)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => t.ProfitLossCurrency ?? 0)
                );
        }

        public async Task<Dictionary<string, object>> GetPerformanceBySetupAsync(string setup)
        {
            if (string.IsNullOrWhiteSpace(setup))
            {
                throw new ArgumentException("Setup cannot be null or whitespace.", nameof(setup));
            }

            var trades = await _context.Trades
                .Where(t => t.SetupType == setup)
                .ToListAsync();

            if (!trades.Any())
            {
                return new Dictionary<string, object>
                {
                    ["TotalTrades"] = 0,
                    ["WinRate"] = 0m,
                    ["TotalProfitLoss"] = 0m,
                    ["AverageRMultiple"] = 0m
                };
            }

            var winningTrades = trades.Count(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value > 0);
            var tradesWithRR = trades.Where(t => t.RealizedRRRatio.HasValue).ToList();

            return new Dictionary<string, object>
            {
                ["TotalTrades"] = trades.Count,
                ["WinRate"] = (decimal)winningTrades / trades.Count * 100,
                ["TotalProfitLoss"] = trades.Sum(t => t.ProfitLossCurrency ?? 0),
                ["AverageRMultiple"] = tradesWithRR.Any() ? tradesWithRR.Average(t => t.RealizedRRRatio!.Value) : 0m
            };
        }

        public async Task<List<(DateTime Date, decimal CumulativeProfitLoss)>> GetEquityCurveAsync()
        {
            var trades = await _context.Trades
                .OrderBy(t => t.EntryDateTime)
                .ToListAsync();

            var equityCurve = new List<(DateTime Date, decimal CumulativeProfitLoss)>();
            decimal runningTotal = 0;

            foreach (var trade in trades)
            {
                runningTotal += trade.ProfitLossCurrency ?? 0;
                equityCurve.Add((trade.EntryDateTime, runningTotal));
            }

            return equityCurve;
        }

        public async Task<Dictionary<string, int>> GetRMultipleDistributionAsync()
        {
            var trades = await _context.Trades.Where(t => t.RealizedRRRatio.HasValue).ToListAsync();

            var distribution = new Dictionary<string, int>
            {
                ["Below -2R"] = 0,
                ["-2R to -1R"] = 0,
                ["-1R to 0R"] = 0,
                ["0R to 1R"] = 0,
                ["1R to 2R"] = 0,
                ["Above 2R"] = 0
            };

            foreach (var trade in trades)
            {
                var rr = trade.RealizedRRRatio!.Value;
                if (rr < -2) distribution["Below -2R"]++;
                else if (rr < -1) distribution["-2R to -1R"]++;
                else if (rr < 0) distribution["-1R to 0R"]++;
                else if (rr < 1) distribution["0R to 1R"]++;
                else if (rr < 2) distribution["1R to 2R"]++;
                else distribution["Above 2R"]++;
            }

            return distribution;
        }

        public async Task<Dictionary<string, decimal>> GetMonthlyPerformanceAsync(int year)
        {
            var trades = await _context.Trades
                .Where(t => t.EntryDateTime.Year == year)
                .ToListAsync();

            var monthlyPerformance = new Dictionary<string, decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthName = new DateTime(year, month, 1).ToString("MMMM");
                var monthTrades = trades.Where(t => t.EntryDateTime.Month == month);
                monthlyPerformance[monthName] = monthTrades.Sum(t => t.ProfitLossCurrency ?? 0);
            }

            return monthlyPerformance;
        }

        public async Task<decimal> GetDisciplineScoreAsync()
        {
            var trades = await _context.Trades.ToListAsync();
            if (!trades.Any())
            {
                return 0;
            }

            // Calculate discipline score based on trades following predefined rules
            var tradesWithSetup = trades.Count(t => !string.IsNullOrEmpty(t.SetupType));
            var tradesWithStopLoss = trades.Count(t => t.StopLoss > 0);
            var tradesWithNotes = trades.Count(t => !string.IsNullOrEmpty(t.Notes));

            var disciplineScore = (decimal)(tradesWithSetup + tradesWithStopLoss + tradesWithNotes) / (trades.Count * 3) * 100;

            return disciplineScore;
        }

        public async Task<Dictionary<string, int>> GetEmotionFrequencyAsync()
        {
            var trades = await _context.Trades.ToListAsync();

            var emotionCounts = new Dictionary<string, int>();

            foreach (var trade in trades)
            {
                if (!string.IsNullOrEmpty(trade.EmotionAtEntry))
                {
                    emotionCounts[trade.EmotionAtEntry] = emotionCounts.GetValueOrDefault(trade.EmotionAtEntry, 0) + 1;
                }
                if (!string.IsNullOrEmpty(trade.EmotionDuringTrade))
                {
                    emotionCounts[trade.EmotionDuringTrade] = emotionCounts.GetValueOrDefault(trade.EmotionDuringTrade, 0) + 1;
                }
                if (!string.IsNullOrEmpty(trade.EmotionAtExit))
                {
                    emotionCounts[trade.EmotionAtExit] = emotionCounts.GetValueOrDefault(trade.EmotionAtExit, 0) + 1;
                }
            }

            return emotionCounts;
        }

        public async Task<List<string>> GetCommonMistakesAsync()
        {
            var trades = await _context.Trades
                .Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value < 0 && !string.IsNullOrEmpty(t.Mistakes))
                .ToListAsync();

            var mistakes = new List<string>();

            // Extract common patterns from losing trades
            var notesWithKeywords = trades
                .Select(t => t.Mistakes!)
                .Where(notes => notes.Contains("mistake", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("should have", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("failed to", StringComparison.OrdinalIgnoreCase));

            mistakes.AddRange(notesWithKeywords);

            return mistakes.Distinct().Take(10).ToList();
        }

        public async Task<List<string>> GetKeyLessonsAsync()
        {
            var trades = await _context.Trades
                .Where(t => !string.IsNullOrEmpty(t.LessonsLearned))
                .ToListAsync();

            return trades
                .Select(t => t.LessonsLearned!)
                .Distinct()
                .Take(10)
                .ToList();
        }

        public async Task<Dictionary<string, decimal>> GetDisciplineCorrelationAsync()
        {
            var tradesWithSetup = await _context.Trades
                .Where(t => !string.IsNullOrEmpty(t.SetupType))
                .ToListAsync();

            var tradesWithoutSetup = await _context.Trades
                .Where(t => string.IsNullOrEmpty(t.SetupType))
                .ToListAsync();

            var avgProfitWithSetup = tradesWithSetup.Any() ? tradesWithSetup.Average(t => t.ProfitLossCurrency ?? 0) : 0;
            var avgProfitWithoutSetup = tradesWithoutSetup.Any() ? tradesWithoutSetup.Average(t => t.ProfitLossCurrency ?? 0) : 0;

            return new Dictionary<string, decimal>
            {
                ["WithSetup"] = avgProfitWithSetup,
                ["WithoutSetup"] = avgProfitWithoutSetup,
                ["Difference"] = avgProfitWithSetup - avgProfitWithoutSetup
            };
        }
    }
}
