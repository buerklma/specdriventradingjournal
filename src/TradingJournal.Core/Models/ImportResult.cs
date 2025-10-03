// <copyright file="ImportResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Core.Models;

/// <summary>
/// Represents the result of a CSV import operation.
/// </summary>
public class ImportResult
{
    /// <summary>
    /// Gets or sets the total number of rows processed.
    /// </summary>
    public int TotalRows { get; set; }

    /// <summary>
    /// Gets or sets the number of successfully imported rows.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Gets or sets the number of rows that failed to import.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the list of error messages encountered during import.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets a value indicating whether the import was successful (no errors).
    /// </summary>
    public bool Success => ErrorCount == 0;
}
