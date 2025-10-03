// <copyright file="ManagementAdjustment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an adjustment applied to an active trade, such as stop-loss or take-profit changes.
    /// </summary>
    public class ManagementAdjustment
    {
    /// <summary>
    /// Gets or sets the unique identifier of the adjustment.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated trade.
    /// </summary>
    [Required]
    public Guid TradeId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the adjustment was made.
    /// </summary>
    [Required]
    public DateTime AdjustmentDateTime { get; set; }

    /// <summary>
    /// Gets or sets the type of the adjustment (e.g., StopLoss, TakeProfit, PartialExit).
    /// </summary>
    [Required]
    public AdjustmentType AdjustmentType { get; set; }

    /// <summary>
    /// Gets or sets the previous value before the adjustment, if applicable.
    /// </summary>
    public decimal? PreviousValue { get; set; }

    /// <summary>
    /// Gets or sets the new value after the adjustment.
    /// </summary>
    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal NewValue { get; set; }

    /// <summary>
    /// Gets or sets the reason for the adjustment.
    /// </summary>
    [MaxLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated trade.
    /// </summary>
    public Trade Trade { get; set; } = null!;
    }
}
