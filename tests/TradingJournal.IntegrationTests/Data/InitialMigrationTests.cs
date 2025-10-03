using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;

namespace TradingJournal.IntegrationTests.Data
{
    public class InitialMigrationTests
    {
        [Fact]
        public void InitialMigration_CreatesAllTables()
        {
            // Arrange
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new TradingDbContext(options);

            // Act
            context.Database.Migrate();

            // Assert
            var tables = new List<string>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            var expectedTables = new[]
            {
                "Trades",
                "ManagementAdjustments",
                "Attachments",
                "StrategyCategories",
                "UserPreferences"
            };

            foreach (var table in expectedTables)
            {
                Assert.Contains(table, tables);
            }
        }
    }
}
