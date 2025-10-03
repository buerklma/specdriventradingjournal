using System;
using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class RiskRewardCalculatorTests
    {
        [Fact]
        public void CalculatePlannedRR_Long_ReturnsCorrectValue()
        {
            decimal entry = 110m;
            decimal stopLoss = 100m;
            decimal takeProfit = 120m;

            decimal result = RiskRewardCalculator.CalculatePlannedRR(entry, stopLoss, takeProfit);
            decimal expected = (takeProfit - entry) / (entry - stopLoss);

            Assert.Equal(expected, result);
        }
    }
}
