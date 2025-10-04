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
            var trades = await _context.Trades
                .AsNoTracking()
                .Select(t => new
                {
                    t.ProfitLossCurrency,
                    t.RealizedRRRatio
                })
                .ToListAsync();

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
            var tradeCount = await _context.Trades.AsNoTracking().CountAsync();
            if (tradeCount == 0)
            {
                return 0;
            }

            var winningTradesCount = await _context.Trades
                .AsNoTracking()
                .CountAsync(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value > 0);

            return (decimal)winningTradesCount / tradeCount * 100;
        }

        public async Task<decimal> GetProfitFactorAsync()
        {
            var profitLossData = await _context.Trades
                .AsNoTracking()
                .Where(t => t.ProfitLossCurrency.HasValue)
                .Select(t => t.ProfitLossCurrency!.Value)
                .ToListAsync();

            var totalWins = profitLossData.Where(pl => pl > 0).Sum();
            var totalLosses = Math.Abs(profitLossData.Where(pl => pl < 0).Sum());

            if (totalLosses == 0)
            {
                return totalWins > 0 ? decimal.MaxValue : 0;
            }

            return totalWins / totalLosses;
        }

        public async Task<decimal> GetAverageRMultipleAsync()
        {
            var rrRatios = await _context.Trades
                .AsNoTracking()
                .Where(t => t.RealizedRRRatio.HasValue)
                .Select(t => t.RealizedRRRatio!.Value)
                .ToListAsync();

            if (!rrRatios.Any())
            {
                return 0;
            }

            return rrRatios.Average();
        }

        public async Task<decimal> GetMaxDrawdownAsync()
        {
            var profitLossData = await _context.Trades
                .AsNoTracking()
                .OrderBy(t => t.EntryDateTime)
                .Select(t => t.ProfitLossCurrency ?? 0)
                .ToListAsync();

            if (!profitLossData.Any())
            {
                return 0;
            }

            decimal runningTotal = 0;
            decimal peak = 0;
            decimal maxDrawdown = 0;

            foreach (var profitLoss in profitLossData)
            {
                runningTotal += profitLoss;
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
            var trades = await _context.Trades
                .AsNoTracking()
                .Select(t => new { t.Symbol, t.ProfitLossCurrency })
                .ToListAsync();

            return trades
                .GroupBy(t => t.Symbol)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(t => t.ProfitLossCurrency ?? 0)
                );
        }

        public async Task<Dictionary<string, decimal>> GetStatisticsBySetupAsync()
        {
            var trades = await _context.Trades
                .AsNoTracking()
                .Where(t => !string.IsNullOrEmpty(t.SetupType))
                .Select(t => new { t.SetupType, t.ProfitLossCurrency })
                .ToListAsync();

            return trades
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
                .AsNoTracking()
                .Where(t => t.SetupType == setup)
                .Select(t => new { t.ProfitLossCurrency, t.RealizedRRRatio })
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
                .AsNoTracking()
                .OrderBy(t => t.EntryDateTime)
                .Select(t => new { t.EntryDateTime, t.ProfitLossCurrency })
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
            var rrRatios = await _context.Trades
                .AsNoTracking()
                .Where(t => t.RealizedRRRatio.HasValue)
                .Select(t => t.RealizedRRRatio!.Value)
                .ToListAsync();

            var distribution = new Dictionary<string, int>
            {
                ["Below -2R"] = 0,
                ["-2R to -1R"] = 0,
                ["-1R to 0R"] = 0,
                ["0R to 1R"] = 0,
                ["1R to 2R"] = 0,
                ["Above 2R"] = 0
            };

            foreach (var rr in rrRatios)
            {
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
                .AsNoTracking()
                .Where(t => t.EntryDateTime.Year == year)
                .Select(t => new { t.EntryDateTime.Month, t.ProfitLossCurrency })
                .ToListAsync();

            var monthlyPerformance = new Dictionary<string, decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthName = new DateTime(year, month, 1).ToString("MMMM");
                var monthTrades = trades.Where(t => t.Month == month);
                monthlyPerformance[monthName] = monthTrades.Sum(t => t.ProfitLossCurrency ?? 0);
            }

            return monthlyPerformance;
        }

        public async Task<decimal> GetDisciplineScoreAsync()
        {
            var disciplineScores = await _context.Trades
                .AsNoTracking()
                .Where(t => t.DisciplineScore.HasValue)
                .Select(t => t.DisciplineScore!.Value)
                .ToListAsync();

            if (!disciplineScores.Any())
            {
                return 0;
            }

            return (decimal)disciplineScores.Average();
        }

        public async Task<Dictionary<string, int>> GetEmotionFrequencyAsync()
        {
            var trades = await _context.Trades
                .AsNoTracking()
                .Select(t => new { t.EmotionAtEntry, t.EmotionDuringTrade, t.EmotionAtExit })
                .ToListAsync();

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
            var mistakes = await _context.Trades
                .AsNoTracking()
                .Where(t => t.ProfitLossCurrency.HasValue && t.ProfitLossCurrency.Value < 0 && !string.IsNullOrEmpty(t.Mistakes))
                .Select(t => t.Mistakes!)
                .ToListAsync();

            // Extract common patterns from losing trades
            var notesWithKeywords = mistakes
                .Where(notes => notes.Contains("mistake", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("should have", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("failed to", StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .Take(10)
                .ToList();

            return notesWithKeywords;
        }

        public async Task<List<string>> GetKeyLessonsAsync()
        {
            var lessons = await _context.Trades
                .AsNoTracking()
                .Where(t => !string.IsNullOrEmpty(t.LessonsLearned))
                .Select(t => t.LessonsLearned!)
                .ToListAsync();

            return lessons
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
