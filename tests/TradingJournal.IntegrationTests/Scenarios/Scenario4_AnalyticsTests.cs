using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;
using TradingJournal.Data.Models;
using TradingJournal.Core.Services;

namespace TradingJournal.IntegrationTests.Scenarios
{
    [Trait("Category", "Scenarios")]
    public class Scenario4_AnalyticsTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TradingDbContext _context;
        private readonly TradeService _tradeService;
        private readonly AnalyticsService _analyticsService;

        public Scenario4_AnalyticsTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TradingDbContext(options);
            _context.Database.EnsureCreated();

            _tradeService = new TradeService(_context);
            _analyticsService = new AnalyticsService(_context);
        }

        [Fact]
        public async Task ThreeTrades_TwoWinsOneLoss_CalculatesCorrectWinRate()
        {
            // Arrange - Create 2 winning trades and 1 losing trade
            var trade1 = await CreateAndExitTrade("AAPL", 100, 110, 2000, TradeDirection.Long, true); // Win
            var trade2 = await CreateAndExitTrade("TSLA", 200, 190, 1000, TradeDirection.Long, false); // Loss
            var trade3 = await CreateAndExitTrade("MSFT", 300, 310, 1500, TradeDirection.Long, true); // Win

            // Act - Calculate analytics
            var winRate = await _analyticsService.GetWinRateAsync();

            // Assert - Win rate should be ~66.67% (2 wins out of 3 trades)
            Assert.True(winRate >= 66.0m && winRate <= 67.0m, $"Expected win rate between 66-67%, got {winRate}");
        }

        [Fact]
        public async Task ThreeTrades_CalculatesTotalPL()
        {
            // Arrange
            await CreateAndExitTrade("AAPL", 100, 110, 100, TradeDirection.Long, true); // +1000
            await CreateAndExitTrade("TSLA", 200, 190, 50, TradeDirection.Long, false); // -500
            await CreateAndExitTrade("MSFT", 300, 310, 75, TradeDirection.Long, true); // +750

            // Act
            var statistics = await _analyticsService.GetOverallStatisticsAsync();

            // Assert - Total P/L should be +1250 (1000 - 500 + 750)
            Assert.NotNull(statistics);
            Assert.True(statistics.ContainsKey("TotalProfitLoss"));
            Assert.True(statistics.ContainsKey("TotalTrades"));
            Assert.Equal(3, Convert.ToInt32(statistics["TotalTrades"]));
            Assert.Equal(2, Convert.ToInt32(statistics["WinningTrades"]));
            Assert.Equal(1, Convert.ToInt32(statistics["LosingTrades"]));
        }

        [Fact]
        public async Task EquityCurve_OrderedByDate()
        {
            // Arrange - Create trades over time
            await CreateAndExitTrade("AAPL", 100, 110, 100, TradeDirection.Long, true);
            await Task.Delay(100); // Ensure different timestamps
            await CreateAndExitTrade("TSLA", 200, 190, 50, TradeDirection.Long, false);
            await Task.Delay(100);
            await CreateAndExitTrade("MSFT", 300, 310, 75, TradeDirection.Long, true);

            // Act
            var equityCurve = await _analyticsService.GetEquityCurveAsync();

            // Assert
            Assert.NotNull(equityCurve);
            Assert.Equal(3, equityCurve.Count);

            // Verify ordered by date
            for (int i = 1; i < equityCurve.Count; i++)
            {
                Assert.True(equityCurve[i].Date >= equityCurve[i - 1].Date);
            }

            // Verify cumulative P/L
            Assert.True(equityCurve.Last().CumulativeProfitLoss > 0);
        }

        private async Task<Trade> CreateAndExitTrade(
            string symbol,
            decimal entryPrice,
            decimal exitPrice,
            decimal positionSize,
            TradeDirection direction,
            bool isWin)
        {
            var stopLoss = direction == TradeDirection.Long
                ? entryPrice - 5m
                : entryPrice + 5m;

            var takeProfit = direction == TradeDirection.Long
                ? entryPrice + 10m
                : entryPrice - 10m;

            var trade = new Trade
            {
                Symbol = symbol,
                Direction = direction,
                EntryDateTime = DateTime.UtcNow.AddHours(-4),
                EntryPrice = entryPrice,
                StopLoss = stopLoss,
                TakeProfit = takeProfit,
                PositionSize = positionSize,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Test",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                exitPrice,
                isWin ? "Take profit" : "Stop loss");

            return createdTrade;
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
