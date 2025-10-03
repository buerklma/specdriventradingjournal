// <copyright file="CsvMappingProfile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Core.Models;

/// <summary>
/// Defines column mappings for CSV import operations.
/// </summary>
public class CsvMappingProfile
{
    /// <summary>
    /// Gets or sets a value indicating whether the CSV file has a header row.
    /// </summary>
    public bool HasHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the column index for the symbol/ticker.
    /// </summary>
    public int SymbolColumn { get; set; }

    /// <summary>
    /// Gets or sets the column index for the trade direction.
    /// </summary>
    public int DirectionColumn { get; set; }

    /// <summary>
    /// Gets or sets the column index for the entry date.
    /// </summary>
    public int EntryDateColumn { get; set; }

    /// <summary>
    /// Gets or sets the column index for the entry price.
    /// </summary>
    public int EntryPriceColumn { get; set; }

    /// <summary>
    /// Gets or sets the column index for the number of shares/position size.
    /// </summary>
    public int SharesColumn { get; set; }

    /// <summary>
    /// Gets or sets the column index for the setup type/pattern.
    /// </summary>
    public int SetupTypeColumn { get; set; }
}
