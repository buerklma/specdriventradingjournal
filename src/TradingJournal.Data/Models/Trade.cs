namespace TradingJournal.Data.Models
{
    public class Trade
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = null!;
        public TradeDirection Direction { get; set; }
        public DateTime EntryDateTime { get; set; }
        public DateTime? ExitDateTime { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal? ExitPrice { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal PositionSize { get; set; }
        public decimal RiskPercentage { get; set; }
        public string Timeframe { get; set; } = null!;
        public string SetupType { get; set; } = null!;
        public string? MarketStructure { get; set; }
        public decimal PlannedRRRatio { get; set; }
        public decimal? RealizedRRRatio { get; set; }
        public decimal? ProfitLossCurrency { get; set; }
        public decimal? ProfitLossR { get; set; }
        public TimeSpan? HoldingTime { get; set; }
        public string? EmotionAtEntry { get; set; }
        public string? EmotionDuringTrade { get; set; }
        public string? EmotionAtExit { get; set; }
        public int? DisciplineScore { get; set; }
        public string? Notes { get; set; }
        public int? QualityRating { get; set; }
        public string? Mistakes { get; set; }
        public string? LessonsLearned { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ManagementAdjustment> Adjustments { get; set; } = new List<ManagementAdjustment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
