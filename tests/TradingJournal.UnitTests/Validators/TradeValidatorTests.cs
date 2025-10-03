using Xunit;
using TradingJournal.Models;
using TradingJournal.Core.Validators;

namespace TradingJournal.UnitTests.Validators
{
    public class TradeValidatorTests
    {
        [Fact]
        public void MissingRequiredFields_ShouldFailValidation()
        {
            var trade = new Trade();
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Symbol");
            Assert.Contains(result.Errors, e => e.PropertyName == "EntryDateTime");
        }

        [Fact]
        public void SymbolTooLong_ShouldFailValidation()
        {
            var trade = new Trade { Symbol = new string('A', 21) };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Symbol");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void PricesMustBePositive_ShouldFailValidation(decimal price)
        {
            var trade = new Trade { EntryPrice = price, StopLoss = price, TakeProfit = price };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EntryPrice");
        }

        [Theory]
        [InlineData(0.05)]
        [InlineData(101)]
        public void RiskPercentageOutOfRange_ShouldFailValidation(decimal risk)
        {
            var trade = new Trade { RiskPercentage = risk };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "RiskPercentage");
        }
    }
}
