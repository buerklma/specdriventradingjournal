using System;
using System.ComponentModel.DataAnnotations;

namespace TradingJournal.Data.Models
{
    /// <summary>
    /// Represents an adjustment applied to an active trade, such as stop-loss or take-profit changes.
    /// </summary>
    public class ManagementAdjustment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TradeId { get; set; }

        [Required]
        public DateTime AdjustmentDateTime { get; set; }

        [Required]
        public AdjustmentType AdjustmentType { get; set; }

        public decimal? PreviousValue { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue)]
        public decimal NewValue { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public Trade Trade { get; set; } = null!;
    }
}
