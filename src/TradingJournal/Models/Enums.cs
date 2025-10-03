// <copyright file="Enums.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    /// <summary>
    /// Specifies the direction of a trade.
    /// </summary>
    public enum TradeDirection
    {
        /// <summary>
        /// Long position.
        /// </summary>
        Long,

        /// <summary>
        /// Short position.
        /// </summary>
        Short,
    }

    /// <summary>
    /// Specifies types of management adjustments for a trade.
    /// </summary>
    public enum AdjustmentType
    {
        /// <summary>
        /// Stop-loss adjustment.
        /// </summary>
        StopLoss,

        /// <summary>
        /// Take-profit adjustment.
        /// </summary>
        TakeProfit,

        /// <summary>
        /// Partial exit adjustment.
        /// </summary>
        PartialExit,
    }

    /// <summary>
    /// Specifies the application theme mode preference.
    /// </summary>
    public enum ThemeMode
    {
        /// <summary>
        /// Light theme.
        /// </summary>
        Light,

        /// <summary>
        /// Dark theme.
        /// </summary>
        Dark,

        /// <summary>
        /// System theme (follows OS setting).
        /// </summary>
        System,
    }
}
