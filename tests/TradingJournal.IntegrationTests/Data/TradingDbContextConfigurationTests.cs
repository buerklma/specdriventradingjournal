using System;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;
using TradingJournal.Data.Models;

namespace TradingJournal.IntegrationTests.Data
{
    public class TradingDbContextConfigurationTests
    {
        [Fact]
        public void IndexesAreConfigured()
        {
            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase("ConfigTestDb")
                .Options;

            using var context = new TradingDbContext(options);
            var model = context.Model;
            Assert.Contains(model.GetEntityTypes(), e => e.FindIndex(new[] { e.FindProperty("Symbol") }) != null);
            Assert.Contains(model.GetEntityTypes(), e => e.FindIndex(new[] { e.FindProperty("EntryDateTime") }) != null);
        }
    }
}
