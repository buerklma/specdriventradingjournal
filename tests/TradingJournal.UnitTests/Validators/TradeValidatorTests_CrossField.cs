using System;
using Xunit;
using TradingJournal.Data.Models;
using TradingJournal.Core.Validators;

namespace TradingJournal.UnitTests.Validators
{
    public class TradeValidatorTests_CrossField
    {
        [Fact]
        public void ExitDateTimeBeforeEntry_ShouldFailValidation()
        {
            var entry = DateTime.UtcNow;
            var exit = entry.AddHours(-1);
            var trade = new Trade
            {
                EntryDateTime = entry,
                ExitDateTime = exit
            };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ExitDateTime");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void DisciplineScoreOutOfRange_ShouldFailValidation(int score)
        {
            var trade = new Trade { DisciplineScore = score };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "DisciplineScore");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void QualityRatingOutOfRange_ShouldFailValidation(int rating)
        {
            var trade = new Trade { QualityRating = rating };
            var validator = new TradeValidator();
            var result = validator.Validate(trade);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "QualityRating");
        }
    }
}
