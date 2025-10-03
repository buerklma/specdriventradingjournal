using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;
using TradingJournal.Data.Models;

namespace TradingJournal.IntegrationTests.Data
{
    public class SeedDataTests
    {
        [Fact]
        public void DefaultEntitiesAreSeeded()
        {
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase("SeedTestDb")
                .Options;

            using var context = new TradingDbContext(options);
            context.Database.EnsureCreated();

            Assert.True(context.StrategyCategories.Any());
            Assert.NotNull(context.UserPreferences.SingleOrDefault());
        }
    }
}
