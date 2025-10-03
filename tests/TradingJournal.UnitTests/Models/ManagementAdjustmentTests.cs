using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Models;

namespace TradingJournal.UnitTests.Models
{
    public class ManagementAdjustmentTests
    {
        [Fact]
        public void CanInstantiateManagementAdjustment()
        {
            var adjustment = new ManagementAdjustment
            {
                TradeId = Guid.NewGuid(),
                AdjustmentDateTime = DateTime.UtcNow,
                AdjustmentType = AdjustmentType.StopLoss,
                NewValue = 150m
            };

            Assert.Equal(AdjustmentType.StopLoss, adjustment.AdjustmentType);
            Assert.Equal(150m, adjustment.NewValue);
        }

        [Fact]
        public void ValidationAttributes_WorkAsExpected()
        {
            var adjustment = new ManagementAdjustment();
            var context = new ValidationContext(adjustment);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(adjustment, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("TradeId"));
            Assert.Contains(results, r => r.MemberNames.Contains("AdjustmentDateTime"));
            Assert.Contains(results, r => r.MemberNames.Contains("AdjustmentType"));
            Assert.Contains(results, r => r.MemberNames.Contains("NewValue"));
        }
    }
}
