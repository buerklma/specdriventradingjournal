using System;
using System.ComponentModel.DataAnnotations;

namespace TradingJournal.Data.Models
{
    public class StrategyCategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string ColorHex { get; set; } = null!;

        public bool IsSystemDefined { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
