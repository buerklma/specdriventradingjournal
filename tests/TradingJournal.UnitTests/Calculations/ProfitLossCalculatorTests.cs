using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class ProfitLossCalculatorTests
    {
        [Fact]
        public void CalculateProfitLossCurrency_Long_ReturnsCorrectValue()
        {
            decimal exitPrice = 120m;
            decimal entryPrice = 100m;
            decimal positionSize = 2m;

            decimal result = ProfitLossCalculator.CalculateProfitLossCurrency(exitPrice, entryPrice, positionSize, ProfitLossCalculator.Direction.Long);
            decimal expected = (exitPrice - entryPrice) * positionSize;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateProfitLossCurrency_Short_ReturnsCorrectValue()
        {
            decimal exitPrice = 80m;
            decimal entryPrice = 100m;
            decimal positionSize = 3m;

            decimal result = ProfitLossCalculator.CalculateProfitLossCurrency(exitPrice, entryPrice, positionSize, ProfitLossCalculator.Direction.Short);
            decimal expected = (entryPrice - exitPrice) * positionSize;

            Assert.Equal(expected, result);
        }
    }
}
