using System;
using Xunit;
using TradingJournal.Core.Calculations;

namespace TradingJournal.UnitTests.Calculations
{
    public class HoldingTimeCalculatorTests
    {
        [Fact]
        public void CalculateHoldingTime_ReturnsCorrectTimeSpan()
        {
            DateTime entry = DateTime.UtcNow;
            // simulate 2 hours later
            DateTime exit = entry.AddHours(2);
            TimeSpan result = HoldingTimeCalculator.CalculateHoldingTime(entry, exit);
            TimeSpan expected = exit - entry;

            Assert.Equal(expected, result);
        }
    }
}
