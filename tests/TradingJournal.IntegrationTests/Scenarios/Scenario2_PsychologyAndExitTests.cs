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
    public class Scenario2_PsychologyAndExitTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TradingDbContext _context;
        private readonly TradeService _tradeService;

        public Scenario2_PsychologyAndExitTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TradingDbContext(options);
            _context.Database.EnsureCreated();

            _tradeService = new TradeService(_context);
        }

        [Fact]
        public async Task PsychologyNotes_AddToOpenTrade_Success()
        {
            // Arrange - Create an open trade
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-2),
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Add psychology notes (simulating PsychologyViewModel)
            createdTrade.EmotionAtEntry = "Confident";
            createdTrade.EmotionDuringTrade = "Anxious";
            createdTrade.DisciplineScore = 8;
            createdTrade.Notes = "Followed my plan, waited for confirmation";

            await _tradeService.UpdateTradeAsync(createdTrade);

            // Assert
            var updatedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            Assert.Equal("Confident", updatedTrade.EmotionAtEntry);
            Assert.Equal("Anxious", updatedTrade.EmotionDuringTrade);
            Assert.Equal(8, updatedTrade.DisciplineScore);
            Assert.Equal("Followed my plan, waited for confirmation", updatedTrade.Notes);
        }

        [Fact]
        public async Task ExitTrade_Win_CalculatesRealizedRRAndPL()
        {
            // Arrange - Create and close a winning trade
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-4),
                EntryPrice = 100.00m,
                StopLoss = 95.00m,
                TakeProfit = 110.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Exit trade at take profit (winning trade)
            var exitDateTime = DateTime.UtcNow;
            var exitPrice = 110.00m;

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                exitDateTime,
                exitPrice,
                "Hit take profit target");

            // Assert - Verify calculated fields
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);

            Assert.NotNull(exitedTrade.ExitDateTime);
            Assert.Equal(exitDateTime, exitedTrade.ExitDateTime.Value, TimeSpan.FromSeconds(1));
            Assert.Equal(exitPrice, exitedTrade.ExitPrice);

            // Verify RealizedRR: (Exit - Entry) / (Entry - SL) = (110 - 100) / (100 - 95) = 2.0
            Assert.Equal(2.0m, exitedTrade.RealizedRRRatio);

            // Verify P/L Currency: (Exit - Entry) * PositionSize = (110 - 100) * 100 = 1000
            Assert.Equal(1000m, exitedTrade.ProfitLossCurrency);

            // Verify P/L in R: RealizedRR = 2.0
            Assert.Equal(2.0m, exitedTrade.ProfitLossR);

            // Verify HoldingTime calculated
            Assert.NotNull(exitedTrade.HoldingTime);
            Assert.True(exitedTrade.HoldingTime.Value.TotalHours >= 4);
        }

        [Fact]
        public async Task ExitTrade_Loss_CalculatesNegativeRRAndPL()
        {
            // Arrange - Create and close a losing trade
            var trade = new Trade
            {
                Symbol = "TSLA",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-3),
                EntryPrice = 200.00m,
                StopLoss = 190.00m,
                TakeProfit = 220.00m,
                PositionSize = 50,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Pullback"
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Exit trade at stop loss (losing trade)
            var exitDateTime = DateTime.UtcNow;
            var exitPrice = 190.00m;

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                exitDateTime,
                exitPrice,
                "Hit stop loss");

            // Assert
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);

            // Verify RealizedRR: (Exit - Entry) / (Entry - SL) = (190 - 200) / (200 - 190) = -1.0
            Assert.Equal(-1.0m, exitedTrade.RealizedRRRatio);

            // Verify P/L Currency: (Exit - Entry) * PositionSize = (190 - 200) * 50 = -500
            Assert.Equal(-500m, exitedTrade.ProfitLossCurrency);

            // Verify P/L in R: -1.0
            Assert.Equal(-1.0m, exitedTrade.ProfitLossR);
        }

        [Fact]
        public async Task ExitTrade_Short_CalculatesCorrectly()
        {
            // Arrange - Short trade
            var trade = new Trade
            {
                Symbol = "NVDA",
                Direction = TradeDirection.Short,
                EntryDateTime = DateTime.UtcNow.AddHours(-2),
                EntryPrice = 500.00m,
                StopLoss = 510.00m, // Above entry for short
                TakeProfit = 480.00m, // Below entry for short
                PositionSize = 20,
                RiskPercentage = 2.0m,
                Timeframe = "1H",
                SetupType = "Reversal"
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Exit at take profit (winning short trade)
            var exitPrice = 480.00m;

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                exitPrice,
                "Take profit hit");

            // Assert
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);

            // Verify RealizedRR for Short: (Entry - Exit) / (SL - Entry) = (500 - 480) / (510 - 500) = 2.0
            Assert.Equal(2.0m, exitedTrade.RealizedRRRatio);

            // Verify P/L Currency for Short: (Entry - Exit) * PositionSize = (500 - 480) * 20 = 400
            Assert.Equal(400m, exitedTrade.ProfitLossCurrency);
        }

        [Fact]
        public async Task PsychologyAndExit_CompleteWorkflow_Success()
        {
            // Arrange - Full scenario
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-5),
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout",
                EmotionAtEntry = "Confident",
                DisciplineScore = 9
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Add during-trade psychology
            createdTrade.EmotionDuringTrade = "Patient";
            await _tradeService.UpdateTradeAsync(createdTrade);

            // Exit the trade
            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                158.00m,
                "Partial profit, strong resistance");

            // Add exit psychology
            var finalTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            finalTrade.EmotionAtExit = "Satisfied";
            await _tradeService.UpdateTradeAsync(finalTrade);

            // Assert - Verify complete workflow
            var completedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);

            Assert.Equal("Confident", completedTrade.EmotionAtEntry);
            Assert.Equal("Patient", completedTrade.EmotionDuringTrade);
            Assert.Equal("Satisfied", completedTrade.EmotionAtExit);
            Assert.Equal(9, completedTrade.DisciplineScore);
            Assert.NotNull(completedTrade.ExitDateTime);
            Assert.NotNull(completedTrade.RealizedRRRatio);
            Assert.NotNull(completedTrade.ProfitLossCurrency);
            Assert.NotNull(completedTrade.HoldingTime);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
