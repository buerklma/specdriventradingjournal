using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;
using TradingJournal.Models;

namespace TradingJournal.IntegrationTests.Data
{
    public class RelationshipTests
    {
        [Fact]
        public void Trade_AdjustmentsRelationshipConfigured()
        {
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase("RelTestDb1")
                .Options;

            using var context = new TradingDbContext(options);
            var trade = new Trade { Id = Guid.NewGuid() };
            context.Trades.Add(trade);
            context.ManagementAdjustments.Add(new ManagementAdjustment { TradeId = trade.Id });
            context.SaveChanges();

            var loaded = context.Trades.Include(t => t.Adjustments).FirstOrDefaultAsync().Result;
            Assert.Single(loaded.Adjustments);
        }

        [Fact]
        public void Trade_AttachmentsRelationshipConfigured()
        {
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase("RelTestDb2")
                .Options;

            using var context = new TradingDbContext(options);
            var trade = new Trade { Id = Guid.NewGuid() };
            context.Trades.Add(trade);
            context.Attachments.Add(new Attachment { TradeId = trade.Id });
            context.SaveChanges();

            var loaded = context.Trades.Include(t => t.Attachments).FirstOrDefaultAsync().Result;
            Assert.Single(loaded.Attachments);
        }
    }
}
