using System;

namespace TradingJournal.Core.Calculations
{
    /// <summary>
    /// Provides methods for calculating profit and loss.
    /// </summary>
    public static class ProfitLossCalculator
    {
        /// <summary>
        /// Enum for trade direction.
        /// </summary>
        public enum Direction
        {
            Long,
            Short
        }

        /// <summary>
        /// Calculates profit or loss in currency for a trade.
        /// </summary>
        /// <param name="exitPrice">The exit price.</param>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="positionSize">The number of units (shares/contracts).</param>
        /// <param name="direction">Trade direction (Long or Short).</param>
        /// <returns>The profit or loss in currency.</returns>
        public static decimal CalculateProfitLossCurrency(decimal exitPrice, decimal entryPrice, decimal positionSize, Direction direction)
        {
            return direction switch
            {
                Direction.Long => (exitPrice - entryPrice) * positionSize,
                Direction.Short => (entryPrice - exitPrice) * positionSize,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid trade direction")
            };
        }

        /// <summary>
        /// Calculates profit or loss in R multiples (profitLossCurrency / risk amount).
        /// </summary>
        /// <param name="profitLossCurrency">The profit or loss in currency.</param>
        /// <param name="riskAmount">The risk amount used (difference between entry and stop-loss).</param>
        /// <returns>The profit or loss in R units.</returns>
        public static decimal CalculateProfitLossR(decimal profitLossCurrency, decimal riskAmount)
        {
            return profitLossCurrency / riskAmount;
        }
    }
}
