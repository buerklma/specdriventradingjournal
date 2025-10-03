using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TradingJournal.Data
{
    /// <summary>
    /// Design-time factory for EF Core migrations.
    /// </summary>
    public class TradingDbContextFactory : IDesignTimeDbContextFactory<TradingDbContext>
    {
        public TradingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TradingDbContext>();

            // Use SQLite database for design-time migrations
            builder.UseSqlite("Data Source=tradingjournal.db");
            return new TradingDbContext(builder.Options);
        }
    }
}
