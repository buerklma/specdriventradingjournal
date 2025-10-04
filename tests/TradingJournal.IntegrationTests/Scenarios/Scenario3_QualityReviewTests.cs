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
    public class Scenario3_QualityReviewTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly TradingDbContext _context;
        private readonly TradeService _tradeService;
        private readonly string _testAttachmentsPath;

        public Scenario3_QualityReviewTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new TradingDbContext(options);
            _context.Database.EnsureCreated();

            _tradeService = new TradeService(_context);

            // Setup test attachments directory
            _testAttachmentsPath = Path.Combine(Path.GetTempPath(), "TradingJournalTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testAttachmentsPath);
        }

        [Fact]
        public async Task QualityReview_AddRatingAndNotes_Success()
        {
            // Arrange - Create and close a trade
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-4),
                EntryPrice = 150.00m,
                StopLoss = 145.00m,
                TakeProfit = 160.00m,
                PositionSize = 100,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Breakout",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                158.00m,
                "Partial profit");

            // Act - Add quality review (simulating ReviewViewModel)
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            exitedTrade!.QualityRating = 8;
            exitedTrade.Mistakes = "Entered too early, didn't wait for full confirmation";
            exitedTrade.LessonsLearned = "Wait for price to close above resistance before entering";

            await _tradeService.UpdateTradeAsync(exitedTrade);

            // Assert
            var reviewedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            Assert.NotNull(reviewedTrade);
            Assert.Equal(8, reviewedTrade.QualityRating);
            Assert.Equal("Entered too early, didn't wait for full confirmation", reviewedTrade.Mistakes);
            Assert.Equal("Wait for price to close above resistance before entering", reviewedTrade.LessonsLearned);
        }

        [Fact]
        public async Task AttachScreenshot_StoreAndRetrieve_Success()
        {
            // Arrange - Create a trade
            var trade = new Trade
            {
                Symbol = "TSLA",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-2),
                EntryPrice = 200.00m,
                StopLoss = 190.00m,
                TakeProfit = 220.00m,
                PositionSize = 50,
                RiskPercentage = 2.0m,
                Timeframe = "1H",
                SetupType = "Pullback",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Create attachment metadata (simulating file upload)
            var testFileName = "test_screenshot.png";
            var testFilePath = Path.Combine(_testAttachmentsPath, testFileName);

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = testFileName,
                FileType = ".png",
                FileSizeBytes = 1024,
                StoragePath = testFilePath,
                Caption = "Trade setup chart",
                UploadedAt = DateTime.UtcNow,
            };

            // Act - Add attachment
            var addedAttachment = await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment);

            // Assert
            Assert.NotNull(addedAttachment);
            Assert.Equal(testFileName, addedAttachment.FileName);
            Assert.Equal(".png", addedAttachment.FileType);
            Assert.Equal(1024, addedAttachment.FileSizeBytes);
            Assert.Equal("Trade setup chart", addedAttachment.Caption);

            // Verify attachment stored in database
            var attachments = await _tradeService.GetAttachmentsForTradeAsync(createdTrade.Id);
            Assert.Single(attachments);
            Assert.Equal(addedAttachment.Id, attachments.First().Id);
        }

        [Fact]
        public async Task MultipleAttachments_AddAndRetrieve_Success()
        {
            // Arrange
            var trade = new Trade
            {
                Symbol = "NVDA",
                Direction = TradeDirection.Short,
                EntryDateTime = DateTime.UtcNow.AddHours(-3),
                EntryPrice = 500.00m,
                StopLoss = 510.00m,
                TakeProfit = 480.00m,
                PositionSize = 20,
                RiskPercentage = 2.0m,
                Timeframe = "4H",
                SetupType = "Reversal",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            // Act - Add multiple attachments
            var attachment1 = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "setup.png",
                FileType = ".png",
                FileSizeBytes = 2048,
                StoragePath = Path.Combine(_testAttachmentsPath, "setup.png"),
                Caption = "Setup chart",
                UploadedAt = DateTime.UtcNow,
            };

            var attachment2 = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "exit.jpg",
                FileType = ".jpg",
                FileSizeBytes = 3072,
                StoragePath = Path.Combine(_testAttachmentsPath, "exit.jpg"),
                Caption = "Exit chart",
                UploadedAt = DateTime.UtcNow,
            };

            var attachment3 = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "notes.txt",
                FileType = ".txt",
                FileSizeBytes = 512,
                StoragePath = Path.Combine(_testAttachmentsPath, "notes.txt"),
                Caption = "Trade notes",
                UploadedAt = DateTime.UtcNow,
            };

            await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment1);
            await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment2);
            await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment3);

            // Assert
            var attachments = (await _tradeService.GetAttachmentsForTradeAsync(createdTrade.Id)).ToList();
            Assert.Equal(3, attachments.Count);
            Assert.Contains(attachments, a => a.FileName == "setup.png");
            Assert.Contains(attachments, a => a.FileName == "exit.jpg");
            Assert.Contains(attachments, a => a.FileName == "notes.txt");
        }

        [Fact]
        public async Task DeleteAttachment_RemovesFileAndRecord_Success()
        {
            // Arrange
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
                SetupType = "Breakout",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "temp.png",
                FileType = ".png",
                FileSizeBytes = 1024,
                StoragePath = Path.Combine(_testAttachmentsPath, "temp.png"),
                Caption = "Temporary file",
                UploadedAt = DateTime.UtcNow,
            };

            var addedAttachment = await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment);

            // Act - Delete attachment
            var result = await _tradeService.DeleteAttachmentAsync(addedAttachment.Id);

            // Assert
            Assert.True(result);

            // Verify removed from database
            var attachments = await _tradeService.GetAttachmentsForTradeAsync(createdTrade.Id);
            Assert.Empty(attachments);
        }

        [Fact]
        public async Task QualityReviewWithAttachment_CompleteWorkflow_Success()
        {
            // Arrange - Create and exit trade
            var trade = new Trade
            {
                Symbol = "MSFT",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow.AddHours(-6),
                EntryPrice = 300.00m,
                StopLoss = 295.00m,
                TakeProfit = 310.00m,
                PositionSize = 50,
                RiskPercentage = 1.5m,
                Timeframe = "1D",
                SetupType = "Trend Continuation",
            };

            var createdTrade = await _tradeService.CreateTradeAsync(trade);

            await _tradeService.UpdateTradeExitAsync(
                createdTrade.Id,
                DateTime.UtcNow,
                308.00m,
                "Near take profit");

            // Act - Add quality review and attachment
            var exitedTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            exitedTrade!.QualityRating = 9;
            exitedTrade.Mistakes = "None - excellent execution";
            exitedTrade.LessonsLearned = "Trend continuation setups work well in strong markets";
            await _tradeService.UpdateTradeAsync(exitedTrade);

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = "trade_chart.png",
                FileType = ".png",
                FileSizeBytes = 2048,
                StoragePath = Path.Combine(_testAttachmentsPath, "trade_chart.png"),
                Caption = "Final trade chart",
                UploadedAt = DateTime.UtcNow,
            };

            await _tradeService.AddAttachmentAsync(createdTrade.Id, attachment);

            // Assert - Verify complete trade
            var finalTrade = await _tradeService.GetTradeByIdAsync(createdTrade.Id);
            Assert.NotNull(finalTrade);
            Assert.Equal(9, finalTrade.QualityRating);
            Assert.NotNull(finalTrade.Mistakes);
            Assert.NotNull(finalTrade.LessonsLearned);
            Assert.NotNull(finalTrade.ExitDateTime);
            Assert.NotNull(finalTrade.RealizedRRRatio);

            var attachments = (await _tradeService.GetAttachmentsForTradeAsync(createdTrade.Id)).ToList();
            Assert.Single(attachments);
            Assert.Equal("trade_chart.png", attachments.First().FileName);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            // Cleanup test directory
            if (Directory.Exists(_testAttachmentsPath))
            {
                Directory.Delete(_testAttachmentsPath, true);
            }
        }
    }
}
