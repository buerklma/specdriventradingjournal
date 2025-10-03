using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Models;

namespace TradingJournal.UnitTests.Models
{
    public class UserPreferencesTests
    {
        [Fact]
        public void SingletonPattern_ShouldReturnSameInstance()
        {
            var instance1 = UserPreferences.Instance;
            var instance2 = UserPreferences.Instance;

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void DefaultValues_ShouldBeInitializedCorrectly()
        {
            var prefs = UserPreferences.Instance;

            Assert.NotNull(prefs.Theme);
            Assert.True(prefs.DefaultRiskPercentage > 0);
            Assert.NotNull(prefs.DefaultCurrency);
        }

        [Fact]
        public void CustomFieldDefinitions_ShouldValidateJsonFormat()
        {
            var prefs = new UserPreferences
            {
                CustomFieldDefinitions = "{ \"Field1\": \"value\" }"
            };
            var context = new ValidationContext(prefs);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(prefs, context, results, true);

            Assert.True(isValid);
        }
    }
}
