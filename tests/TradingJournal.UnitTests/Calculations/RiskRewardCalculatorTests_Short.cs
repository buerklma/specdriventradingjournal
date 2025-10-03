using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class RiskRewardCalculatorTests_Short
    {
        [Fact]
        public void CalculatePlannedRR_Short_ReturnsCorrectValue()
        {
            decimal entry = 110m;
            decimal stopLoss = 120m;
            decimal takeProfit = 100m;

            decimal result = RiskRewardCalculator.CalculatePlannedRR(entry, stopLoss, takeProfit);
            decimal expected = (entry - takeProfit) / (stopLoss - entry);

            Assert.Equal(expected, result);
        }
    }
}
