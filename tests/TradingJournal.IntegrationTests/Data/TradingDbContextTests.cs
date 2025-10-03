using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;

namespace TradingJournal.IntegrationTests.Data
{
    public class TradingDbContextTests
    {
        [Fact]
        public void CanInstantiateDbContext_WithInMemoryOptions()
        {
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new TradingDbContext(options);
            Assert.NotNull(context.Trades);
            Assert.NotNull(context.ManagementAdjustments);
            Assert.NotNull(context.Attachments);
        }
    }
}
