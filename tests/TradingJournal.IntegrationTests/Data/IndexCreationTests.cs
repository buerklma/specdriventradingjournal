using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TradingJournal.Data;

namespace TradingJournal.IntegrationTests.Data
{
    public class IndexCreationTests
    {
        [Fact]
        public void Migration_CreatesAllIndexes()
        {
            // Arrange: in-memory SQLite
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TradingDbContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new TradingDbContext(options);

            // Act
            context.Database.Migrate();

            // Assert
            var indexes = new List<string>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='index';";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    indexes.Add(reader.GetString(0));
                }
            }

            var expected = new[]
            {
                "IX_Trade_Symbol",
                "IX_Trade_EntryDateTime",
                "IX_Trade_SetupType",
                "IX_Trade_Symbol_EntryDateTime",
            };

            foreach (var idx in expected)
            {
                Assert.Contains(idx, indexes);
            }
        }
    }
}
