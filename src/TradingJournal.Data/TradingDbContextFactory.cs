using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TradingJournal.Data
{
    public class TradingDbContextFactory : IDesignTimeDbContextFactory<TradingDbContext>
    {
        public TradingDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TradingDbContext>();
            builder.UseSqlite("Data Source=tradingjournal.db");
            return new TradingDbContext(builder.Options);
        }
    }
}
