using System;

namespace TradingJournal.Core.Calculations
{
    /// <summary>
    /// Provides methods for calculating holding time of a trade.
    /// </summary>
    public static class HoldingTimeCalculator
    {
        /// <summary>
        /// Calculates the time span between entry and exit.
        /// </summary>
        /// <param name="entryDateTime">Trade entry timestamp.</param>
        /// <param name="exitDateTime">Trade exit timestamp.</param>
        /// <returns>TimeSpan representing holding duration.</returns>
        public static TimeSpan CalculateHoldingTime(DateTime entryDateTime, DateTime exitDateTime)
        {
            return exitDateTime - entryDateTime;
        }
    }
}
