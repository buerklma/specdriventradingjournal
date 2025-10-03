namespace TradingJournal.Core.Calculations
{
    /// <summary>
    /// Provides methods for calculating risk/reward ratios.
    /// </summary>
    public static class RiskRewardCalculator
    {
        /// <summary>
        /// Calculates the planned risk/reward ratio for a trade.
        /// </summary>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="stopLoss">The stop-loss price.</param>
        /// <param name="takeProfit">The take-profit price.</param>
        /// <returns>The planned R/R ratio.</returns>
        public static decimal CalculatePlannedRR(decimal entryPrice, decimal stopLoss, decimal takeProfit)
        {
            return (takeProfit - entryPrice) / (entryPrice - stopLoss);
        }

        /// <summary>
        /// Calculates the realized risk/reward ratio for a trade.
        /// </summary>
        /// <param name="exitPrice">The exit price.</param>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="riskAmount">The risk amount (entry price minus stop-loss for long, or stop-loss minus entry price for short).</param>
        /// <returns>The realized R/R ratio.</returns>
        public static decimal CalculateRealizedRR(decimal exitPrice, decimal entryPrice, decimal riskAmount)
        {
            return (exitPrice - entryPrice) / riskAmount;
        }
    }
}
