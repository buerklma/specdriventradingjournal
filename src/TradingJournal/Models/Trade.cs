// <copyright file="Trade.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a trading transaction with all details, psychology, and review data.
    /// </summary>
    public class Trade
    {
    /// <summary>
    /// Gets or sets the unique identifier of the trade.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol (max 20 chars).
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; } = null!;

    /// <summary>
    /// Gets or sets the direction of the trade (Long or Short).
    /// </summary>
    [Required]
    public TradeDirection Direction { get; set; }

    /// <summary>
    /// Gets or sets when the trade was entered.
    /// </summary>
    [Required]
    public DateTime EntryDateTime { get; set; }

    /// <summary>
    /// Gets or sets when the trade was exited.
    /// </summary>
    public DateTime? ExitDateTime { get; set; }

        /// <summary>
        /// Gets or sets the entry price.
        /// </summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal EntryPrice { get; set; }

    /// <summary>
    /// Gets or sets the exit price.
    /// </summary>
    public decimal? ExitPrice { get; set; }

        /// <summary>
        /// Gets or sets the stop-loss price.
        /// </summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal StopLoss { get; set; }

        /// <summary>
        /// Gets or sets the take-profit target.
        /// </summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal TakeProfit { get; set; }

        /// <summary>
        /// Gets or sets the position size.
        /// </summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal PositionSize { get; set; }

        /// <summary>
        /// Gets or sets the risk percentage (0.1 to 100).
        /// </summary>
    [Required]
    [Range(0.1, 100)]
    public decimal RiskPercentage { get; set; }

        /// <summary>
        /// Gets or sets the chart timeframe (max 10 chars).
        /// </summary>
    [Required]
    [MaxLength(10)]
    public string Timeframe { get; set; } = null!;

        /// <summary>
        /// Gets or sets the setup type (max 50 chars).
        /// </summary>
    [Required]
    [MaxLength(50)]
    public string SetupType { get; set; } = null!;

    /// <summary>
    /// Gets or sets market structure notes (max 200 chars).
    /// </summary>
    [MaxLength(200)]
    public string? MarketStructure { get; set; }

    /// <summary>
    /// Gets or sets the planned risk/reward ratio.
    /// </summary>
    [Required]
    public decimal PlannedRRRatio { get; set; }

    /// <summary>
    /// Gets or sets the realized risk/reward ratio.
    /// </summary>
    public decimal? RealizedRRRatio { get; set; }

    /// <summary>
    /// Gets or sets the profit/loss in currency.
    /// </summary>
    public decimal? ProfitLossCurrency { get; set; }

    /// <summary>
    /// Gets or sets the profit/loss in R multiples.
    /// </summary>
    public decimal? ProfitLossR { get; set; }

    /// <summary>
    /// Gets or sets the holding time duration.
    /// </summary>
    public TimeSpan? HoldingTime { get; set; }

    /// <summary>
    /// Gets or sets the emotion at entry (max 100 chars).
    /// </summary>
    [MaxLength(100)]
    public string? EmotionAtEntry { get; set; }

    /// <summary>
    /// Gets or sets the emotion during trade (max 100 chars).
    /// </summary>
    [MaxLength(100)]
    public string? EmotionDuringTrade { get; set; }

    /// <summary>
    /// Gets or sets the emotion at exit (max 100 chars).
    /// </summary>
    [MaxLength(100)]
    public string? EmotionAtExit { get; set; }

    /// <summary>
    /// Gets or sets the discipline score (1-10).
    /// </summary>
    [Range(1, 10)]
    public int? DisciplineScore { get; set; }

    /// <summary>
    /// Gets or sets free-form notes (max 2000 chars).
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the quality rating (1-10).
    /// </summary>
    [Range(1, 10)]
    public int? QualityRating { get; set; }

    /// <summary>
    /// Gets or sets mistakes made (max 1000 chars).
    /// </summary>
    [MaxLength(1000)]
    public string? Mistakes { get; set; }

    /// <summary>
    /// Gets or sets lessons learned (max 1000 chars).
    /// </summary>
    [MaxLength(1000)]
    public string? LessonsLearned { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of management adjustments.
    /// </summary>
    public ICollection<ManagementAdjustment> Adjustments { get; set; } = new List<ManagementAdjustment>();

    /// <summary>
    /// Gets or sets the collection of attachments.
    /// </summary>
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
