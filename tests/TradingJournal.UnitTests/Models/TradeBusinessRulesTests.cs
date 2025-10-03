using System;
using Xunit;
using TradingJournal.Models;

namespace TradingJournal.UnitTests.Models
{
    public class TradeBusinessRulesTests
    {
        [Fact]
        public void LongDirection_ValidLimits_ShouldBeValid()
        {
            var trade = new Trade
            {
                Direction = TradeDirection.Long,
                StopLoss = 100m,
                EntryPrice = 110m,
                TakeProfit = 120m
            };

            Assert.True(trade.StopLoss < trade.EntryPrice && trade.EntryPrice < trade.TakeProfit);
        }

        [Fact]
        public void LongDirection_InvalidLimits_ShouldBeInvalid()
        {
            var trade = new Trade
            {
                Direction = TradeDirection.Long,
                StopLoss = 120m,
                EntryPrice = 110m,
                TakeProfit = 100m
            };

            Assert.False(trade.StopLoss < trade.EntryPrice && trade.EntryPrice < trade.TakeProfit);
        }

        [Fact]
        public void ShortDirection_ValidLimits_ShouldBeValid()
        {
            var trade = new Trade
            {
                Direction = TradeDirection.Short,
                StopLoss = 120m,
                EntryPrice = 110m,
                TakeProfit = 100m
            };

            Assert.True(trade.TakeProfit < trade.EntryPrice && trade.EntryPrice < trade.StopLoss);
        }

        [Fact]
        public void ExitDateTime_AfterEntry_ShouldBeValid()
        {
            var entry = DateTime.UtcNow;
            var exit = entry.AddHours(1);
            var trade = new Trade
            {
                EntryDateTime = entry,
                ExitDateTime = exit
            };

            Assert.True(trade.ExitDateTime > trade.EntryDateTime);
        }
    }
}
