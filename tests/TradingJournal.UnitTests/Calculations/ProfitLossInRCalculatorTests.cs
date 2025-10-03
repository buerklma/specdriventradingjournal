using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class ProfitLossInRCalculatorTests
    {
        [Fact]
        public void CalculateProfitLossR_ReturnsCorrectValue()
        {
            decimal profitLossCurrency = 20m;
            decimal riskAmount = 10m;

            decimal result = ProfitLossCalculator.CalculateProfitLossR(profitLossCurrency: profitLossCurrency, riskAmount: riskAmount);
            decimal expected = profitLossCurrency / riskAmount;

            Assert.Equal(expected, result);
        }
    }
}
