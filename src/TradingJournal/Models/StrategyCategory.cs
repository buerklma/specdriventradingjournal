// <copyright file="StrategyCategory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a strategy category for trades (e.g., Breakout, Pullback).
    /// </summary>
    public class StrategyCategory
    {
        /// <summary>
        /// Gets or sets the unique identifier of the category.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the category.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the color hex code for the category (e.g., #FF0000).
        /// </summary>
        [Required]
        [MaxLength(7)]
        public string ColorHex { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether this category is system-defined.
        /// </summary>
        public bool IsSystemDefined { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of trades associated with this category.
        /// </summary>
        public ICollection<Trade> Trades { get; set; } = new List<Trade>();
    }
}
