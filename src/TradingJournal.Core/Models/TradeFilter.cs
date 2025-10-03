using TradingJournal.Data.Models;

namespace TradingJournal.Core.Models;

/// <summary>
/// Filter criteria for querying trades with various conditions.
/// Used by ITradeService.GetTradesPagedAsync to apply multiple filters.
/// </summary>
public class TradeFilter
{
    /// <summary>
    /// Filter by stock symbol (case-insensitive matching).
    /// </summary>
    public string? Symbol { get; set; }

    /// <summary>
    /// Filter trades with EntryDateTime on or after this date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter trades with EntryDateTime on or before this date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter by setup type (e.g., "Breakout", "Pullback", "Range").
    /// </summary>
    public string? SetupType { get; set; }

    /// <summary>
    /// Filter by trade direction (Long or Short).
    /// </summary>
    public TradeDirection? Direction { get; set; }

    /// <summary>
    /// Filter by open/closed status.
    /// True = only open trades (ExitDateTime == null)
    /// False = only closed trades (ExitDateTime != null)
    /// Null = both open and closed trades
    /// </summary>
    public bool? IsOpen { get; set; }

    /// <summary>
    /// Filter by profitability status (only applies to closed trades).
    /// True = only profitable trades (ProfitLossCurrency > 0)
    /// False = only unprofitable trades (ProfitLossCurrency <= 0)
    /// Null = all trades regardless of profitability
    /// </summary>
    public bool? IsProfitable { get; set; }

    /// <summary>
    /// Checks if any filter criteria is set.
    /// </summary>
    public bool HasFilters =>
        !string.IsNullOrEmpty(Symbol) ||
        StartDate.HasValue ||
        EndDate.HasValue ||
        !string.IsNullOrEmpty(SetupType) ||
        Direction.HasValue ||
        IsOpen.HasValue ||
        IsProfitable.HasValue;
}
