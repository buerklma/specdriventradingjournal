using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Models;

namespace TradingJournal.UnitTests.Models
{
    public class TradeTests
    {
        [Fact]
        public void CanInstantiateAndSetProperties()
        {
            var trade = new Trade
            {
                Symbol = "AAPL",
                Direction = TradeDirection.Long,
                EntryDateTime = DateTime.UtcNow,
                StopLoss = 100m,
                EntryPrice = 110m,
                TakeProfit = 120m
            };

            Assert.Equal("AAPL", trade.Symbol);
            Assert.Equal(TradeDirection.Long, trade.Direction);
            Assert.True(trade.StopLoss < trade.EntryPrice && trade.EntryPrice < trade.TakeProfit);
        }

        [Fact]
        public void ValidationAttributes_WorkAsExpected()
        {
            var trade = new Trade(); // Missing required fields
            var context = new ValidationContext(trade);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(trade, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Symbol"));
            Assert.Contains(results, r => r.MemberNames.Contains("EntryDateTime"));
        }
    }
}
