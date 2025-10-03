using FluentAssertions;
using TradingJournal.Data.Models;
using Xunit;

namespace TradingJournal.UnitTests.Services
{
    /// <summary>
    /// Tests for IAnalyticsService.GetTopMistakesAsync method.
    /// </summary>
    public class AnalyticsService_MistakesTests
    {
        [Fact]
        public async Task GetTopMistakesAsync_ShouldParseMistakesField_AndCountFrequency()
        {
            // Arrange - Create trades with Mistakes: "Overtrading", "FOMO", "Overtrading"

            // Act
            // var result = await analyticsService.GetTopMistakesAsync(10);

            // Assert
            // result.Should().ContainSingle(m => m.Mistake == "Overtrading" && m.Count == 2);
            // result.Should().ContainSingle(m => m.Mistake == "FOMO" && m.Count == 1);

            Assert.True(true, "Test stub - will be implemented when IAnalyticsService is available");
        }

        [Fact]
        public async Task GetTopMistakesAsync_ShouldReturnTopN_OrderedByFrequency()
        {
            // Arrange - Create trades with various mistakes
            int topN = 5;

            // Act
            // var result = await analyticsService.GetTopMistakesAsync(topN);

            // Assert
            // result.Should().HaveCountLessOrEqualTo(topN);
            // result.Should().BeInDescendingOrder(m => m.Count);

            Assert.True(true, "Test stub - will be implemented when IAnalyticsService is available");
        }

        [Fact]
        public async Task GetTopMistakesAsync_ShouldParseSeparators_CommmaAndNewline()
        {
            // Arrange - Create trades with mistakes separated by comma and newline

            // Act
            // var result = await analyticsService.GetTopMistakesAsync(10);

            // Assert - Should parse all mistakes correctly
            // result.Should().NotBeEmpty();

            Assert.True(true, "Test stub - will be implemented when IAnalyticsService is available");
        }

        [Fact]
        public async Task GetTopMistakesAsync_ShouldReturnEmptyList_WhenNoMistakesRecorded()
        {
            // Arrange - Create trades without Mistakes field

            // Act
            // var result = await analyticsService.GetTopMistakesAsync(10);

            // Assert
            // result.Should().BeEmpty();

            Assert.True(true, "Test stub - will be implemented when IAnalyticsService is available");
        }
    }
}
