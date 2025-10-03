using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Data.Models;

namespace TradingJournal.UnitTests.Models
{
    public class StrategyCategoryTests
    {
        [Fact]
        public void CanInstantiateAndSetProperties()
        {
            var category = new StrategyCategory
            {
                Name = "Breakout",
                ColorHex = "#FF0000",
                IsSystemDefined = true
            };

            Assert.Equal("Breakout", category.Name);
            Assert.Equal("#FF0000", category.ColorHex);
            Assert.True(category.IsSystemDefined);
        }

        [Fact]
        public void ValidationAttributes_RequireNameAndMaxLength()
        {
            var category = new StrategyCategory { Name = null };
            var context = new ValidationContext(category);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(category, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("Name"));
        }
    }
}
