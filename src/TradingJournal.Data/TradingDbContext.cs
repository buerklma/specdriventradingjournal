using Microsoft.EntityFrameworkCore;
using TradingJournal.Data.Models;

namespace TradingJournal.Data
{
    public class TradingDbContext : DbContext
    {
        public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }
        public DbSet<Trade> Trades { get; set; } = null!;
        public DbSet<ManagementAdjustment> ManagementAdjustments { get; set; } = null!;
        public DbSet<Attachment> Attachments { get; set; } = null!;
        public DbSet<StrategyCategory> StrategyCategories { get; set; } = null!;
        public DbSet<UserPreferences> UserPreferences { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Trade indexes
            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.Symbol)
                .HasDatabaseName("IX_Trade_Symbol");
            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.EntryDateTime)
                .HasDatabaseName("IX_Trade_EntryDateTime");
            modelBuilder.Entity<Trade>()
                .HasIndex(t => t.SetupType)
                .HasDatabaseName("IX_Trade_SetupType");
            modelBuilder.Entity<Trade>()
                .HasIndex(t => new { t.Symbol, t.EntryDateTime })
                .HasDatabaseName("IX_Trade_Symbol_EntryDateTime");

            // Cascade deletes for related entities
            modelBuilder.Entity<Trade>()
                .HasMany(t => t.Adjustments)
                .WithOne(a => a.Trade)
                .HasForeignKey(a => a.TradeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Trade>()
                .HasMany(t => t.Attachments)
                .WithOne(a => a.Trade)
                .HasForeignKey(a => a.TradeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed default strategy categories
            modelBuilder.Entity<StrategyCategory>().HasData(
                new StrategyCategory
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Name = "Breakout",
                    Description = null,
                    ColorHex = "#FF5733",
                    IsSystemDefined = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new StrategyCategory
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Name = "Pullback",
                    Description = null,
                    ColorHex = "#33FF57",
                    IsSystemDefined = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new StrategyCategory
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Name = "Reversal",
                    Description = null,
                    ColorHex = "#3357FF",
                    IsSystemDefined = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new StrategyCategory
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Name = "Trend Continuation",
                    Description = null,
                    ColorHex = "#FF33A1",
                    IsSystemDefined = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                });

            // Seed default user preferences
            modelBuilder.Entity<UserPreferences>().HasData(
                new UserPreferences
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                    Theme = ThemeMode.System,
                    DefaultCurrency = "USD",
                    DefaultRiskPercentage = 1m,
                    EnableBiometricAuth = false,
                    PinHash = null,
                    DatabaseEncrypted = false,
                    CustomFieldDefinitions = "{}",
                    UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }
}
