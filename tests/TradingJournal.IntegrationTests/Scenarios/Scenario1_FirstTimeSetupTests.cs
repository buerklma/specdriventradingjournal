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
    public class Scenario1_FirstTimeSetupTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TradingDbContext _context;
        private readonly TradeService _tradeService;

        public Scenario1_FirstTimeSetupTests()
        {
            // Create SQLite in-memory database
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
        public async Task FirstTrade_CreateWorkflow_Success()
        {
            // Arrange - Simulate user input from NewTradeViewModel
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            // Act - TradeService.CreateTradeAsync
            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Assert - Verify trade created
            Assert.NotNull(createdTrade);
            Assert.NotEqual(Guid.Empty, createdTrade.Id);

            // Verify DB insert
            var dbTrade = await _context.Trades.FindAsync(createdTrade.Id);
            Assert.NotNull(dbTrade);
            Assert.Equal("AAPL", dbTrade.Symbol);
            Assert.Equal(TradeDirection.Long, dbTrade.Direction);

            // Verify calculated fields
            Assert.NotNull(dbTrade.PlannedRRRatio);
            Assert.True(dbTrade.PlannedRRRatio > 0);

            // Verify timestamps
            Assert.True(dbTrade.CreatedAt != default);
            Assert.True(dbTrade.UpdatedAt != default);
        }

        [Fact]
        public async Task FirstTrade_CalculatedFields_PlannedRRCorrect()
        {
            // Arrange - Long trade: (TP - Entry) / (Entry - SL)
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 100.00m,
                StopLoss = 95.00m,
                TakeProfit = 110.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            // Expected PlannedRR: (110 - 100) / (100 - 95) = 10 / 5 = 2.0
            decimal expectedPlannedRR = 2.0m;

            // Act
            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Assert
            Assert.Equal(expectedPlannedRR, createdTrade.PlannedRRRatio);
        }

        [Fact]
        public async Task FirstTrade_ValidationErrors_ThrowsException()
        {
            // Arrange - Invalid trade (empty symbol)
            var trade = new Trade
            {
                Symbol = "", // Invalid
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _tradeService.CreateTradeAsync(trade);
            });
        }

        [Fact]
        public async Task FirstTrade_NavigationProperties_Loaded()
        {
            // Arrange & Act
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout"
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Retrieve with navigation properties
            var retrievedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);

            // Assert
            Assert.NotNull(retrievedTrade);
            Assert.NotNull(retrievedTrade.Adjustments);
            Assert.NotNull(retrievedTrade.Attachments);
            Assert.Empty(retrievedTrade.Adjustments); // No adjustments yet
            Assert.Empty(retrievedTrade.Attachments); // No attachments yet
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
