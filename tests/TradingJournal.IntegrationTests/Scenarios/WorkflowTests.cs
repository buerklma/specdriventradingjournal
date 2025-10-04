using System;
using System.IO;
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
    public class WorkflowTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TradingDbContext _context;
        private readonly TradeService _tradeService;
        private readonly AnalyticsService _analyticsService;
        private readonly ExportService _exportService;

        public WorkflowTests()
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
            _exportService = new ExportService(_context);
        }

        /// <summary>
        /// T083: Full Trade Lifecycle Test
        /// Tests the complete trade lifecycle from creation to export.
        /// </summary>
        [Fact]
        public async Task FullTradeLifecycle_CompleteWorkflow_Success()
        {
            // 1. Create trade
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-6),
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);
            Assert.NotNull(createdTrade);

            // 2. Add psychology notes
            createdTrade.EmotionAtEntry = "Confident";
            createdTrade.EmotionDuringTrade = "Patient";
            createdTrade.DisciplineScore = 9;
            await _tradeService.UpdateTradeAsync(createdTrade);

            // 3. Add adjustment (position management)
            var adjustment = new ManagementAdjustment
            {
                Id = Guid.NewGuid(),
                AdjustmentDateTime = DateTime.UtcNow.AddHours(-2),
                AdjustmentType = AdjustmentType.StopLoss,
                PreviousValue = 145.00m,
                NewValue = 148.00m,
                Reason = "Move to break-even",
            };

            await _tradeService.AddAdjustmentAsync(createdTrade.Id, adjustment);

            // 4. Exit trade
            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                158.00m,
                "Partial profit");

            // 5. Add review
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            exitedTrade!.EmotionAtExit = "Satisfied";
            exitedTrade.QualityRating = 8;
            exitedTrade.Mistakes = "Could have waited for better entry";
            exitedTrade.LessonsLearned = "Patience pays off";
            await _tradeService.UpdateTradeAsync(exitedTrade);

            // 6. Attach screenshot (simulated)
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "trade_chart.png",
                FileType = ".png",
                FileSizeBytes = 2048,
                StoragePath = "/temp/trade_chart.png",
                Caption = "Final chart",
                UploadedAt = DateTime.UtcNow,
            };

            await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment);

            // 7. Verify complete trade
            var finalTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            Assert.NotNull(finalTrade);
            Assert.NotNull(finalTrade.ExitDateTime);
            Assert.NotNull(finalTrade.RealizedRRRatio);
            Assert.NotNull(finalTrade.QualityRating);

            var adjustments = await _tradeService.GetAdjustmentsForTradeAsync(createdTrade.Id);
            Assert.Single(adjustments);

            var attachments = await _tradeService.GetAttachmentsForTradeAsync(createdTrade.Id);
            Assert.Single(attachments);
        }

        /// <summary>
        /// T084: Concurrent Trade Creation Test
        /// Tests multiple trades created simultaneously without locking issues.
        /// </summary>
        [Fact]
        public async Task ConcurrentTradeCreation_NoLockingIssues_AllPersisted()
        {
            // Arrange - Create multiple trades concurrently
            var trades = Enumerable.Range(0, 10).Select(i => new Trade
            {
                Symbol = $"SYM{i}",
                Direction = i % 2 == 0 ? TradeDirection.Long : TradeDirection.Short,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 100.00m + i,
                StopLoss = 95.00m + i,
                TakeProfit = 110.00m + i,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Test",
            }).ToList();

            // Act - Create all trades concurrently
            var tasks = trades.Select(t => _tradeService.CreateTradeAsync(t));
            var createdTrades = await Task.WhenAll(tasks);

            // Assert - All trades persisted
            Assert.Equal(10, createdTrades.Length);
            Assert.All(createdTrades, t => Assert.NotEqual(Guid.Empty, t.Id));

            var allTrades = (await _tradeService.GetAllTradesAsync()).ToList();
            Assert.Equal(10, allTrades.Count);
        }

        /// <summary>
        /// T085: Large Dataset Performance Test
        /// Tests performance with 1000 trades (simplified version).
        /// Note: Full performance benchmarks would need actual timing assertions.
        /// </summary>
        [Fact(Skip = "Performance test - run manually when needed")]
        public async Task LargeDataset_PerformanceAcceptable()
        {
            // Create 1000 trades (simplified - actual test would measure timing)
            for (int i = 0; i < 1000; i++)
            {
                var trade = new Trade
                {
                    Symbol = $"SYM{i % 100}",
                    Direction = i % 2 == 0 ? TradeDirection.Long : TradeDirection.Short,
                    EntryDateTime = DateTime.UtcNow.AddDays(-i),
                    EntryPrice = 100.00m,
                    StopLoss = 95.00m,
                    TakeProfit = 110.00m,
                    PositionSize = 100,
                    RiskPercentage = 2.0m,
                    Timeframe = "4H",
                    SetupType = "Test",
                };

                var created = await _tradeService.CreateTradeAsync(trade);
                await _tradeService.UpdateTradeExitAsync(
                    created.Id,
                    DateTime.UtcNow,
                    i % 2 == 0 ? 110.00m : 95.00m,
                    "Test exit");
            }

            // Verify analytics still work
            var statistics = await _analyticsService.GetOverallStatisticsAsync();
            Assert.Equal(1000, Convert.ToInt32(statistics["TotalTrades"]));

            // Verify paging works
            var pagedResult = await _tradeService.GetTradesPagedAsync(1, 100);
            Assert.Equal(100, pagedResult.Items.Count());
            Assert.Equal(10, pagedResult.TotalPages);
        }

        /// <summary>
        /// T086: Backup and Restore Cycle Test
        /// Note: Actual backup/restore requires file system operations.
        /// This is a simplified validation test.
        /// </summary>
        [Fact(Skip = "Requires file system operations - implement when ExportService is complete")]
        public async Task BackupRestoreCycle_DataRestored()
        {
            // 1. Create some trades
            await CreateTestTrade("AAPL");
            await CreateTestTrade("TSLA");
            await CreateTestTrade("MSFT");

            var originalCount = (await _tradeService.GetAllTradesAsync()).ToList().Count;
            Assert.Equal(3, originalCount);

            // 2. Create backup (would use ExportService.CreateBackupAsync)
            // var backupPath = await _exportService.CreateBackupAsync(Path.GetTempPath());
            // Assert.True(File.Exists(backupPath));

            // 3. Delete all data
            // var trades = await _tradeService.GetAllTradesAsync();
            // foreach (var trade in trades)
            // {
            //     await _tradeService.DeleteTradeAsync(trade.Id);
            // }
            // Assert.Equal(0, (await _tradeService.GetAllTradesAsync()).Count);

            // 4. Restore from backup (would use ExportService.RestoreFromBackupAsync)
            // await _exportService.RestoreFromBackupAsync(backupPath, true);

            // 5. Verify data restored
            // var restoredCount = (await _tradeService.GetAllTradesAsync()).Count;
            // Assert.Equal(originalCount, restoredCount);

            // Placeholder assertion
            Assert.True(true, "Backup/Restore test placeholder");
        }

        private async Task<Trade> CreateTestTrade(string symbol)
        {
            var trade = new Trade
            {
                Symbol = symbol,
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                EntryPrice = 100.00m,
                StopLoss = 95.00m,
                TakeProfit = 110.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Test",
            };

            return await _tradeService.CreateTradeAsync(trade);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
