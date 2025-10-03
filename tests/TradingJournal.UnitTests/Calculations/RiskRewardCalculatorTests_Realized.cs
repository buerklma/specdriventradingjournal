using System;
using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class RiskRewardCalculatorTests_Realized
    {
        [Theory]
        [InlineData(120, 100, 1.0)]
        [InlineData(100, 100, 0.0)]
        [InlineData(90, 100, -0.1)]
        public void CalculateRealizedRR_ReturnsCorrectValue(decimal exitPrice, decimal entryPrice, double expected)
        {
            decimal result = RiskRewardCalculator.CalculateRealizedRR(exitPrice, entryPrice, 100m);
            Assert.Equal((decimal)expected, result);
        }
    }
}
