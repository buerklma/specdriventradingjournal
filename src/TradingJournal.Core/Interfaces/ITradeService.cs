using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingJournal.Core.Models;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Interfaces
{
    /// <summary>
    /// Service interface for managing trade operations.
    /// </summary>
    public interface ITradeService
    {
        /// <summary>
        /// Creates a new trade in the database.
        /// </summary>
        Task<Trade> CreateTradeAsync(Trade trade);

        /// <summary>
        /// Retrieves a trade by its ID including related entities.
        /// </summary>
        Task<Trade?> GetTradeByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all trades ordered by entry date descending.
        /// </summary>
        Task<IEnumerable<Trade>> GetAllTradesAsync();

        /// <summary>
        /// Retrieves all trades for a specific symbol.
        /// </summary>
        Task<IEnumerable<Trade>> GetTradesBySymbolAsync(string symbol);

        /// <summary>
        /// Retrieves all trades within a date range.
        /// </summary>
        Task<IEnumerable<Trade>> GetTradesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves trades with pagination.
        /// </summary>
        Task<PagedResult<Trade>> GetTradesPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves trades with pagination and filtering.
        /// </summary>
        Task<PagedResult<Trade>> GetTradesPagedAsync(int pageNumber, int pageSize, TradeFilter filter);

        /// <summary>
        /// Updates an existing trade.
        /// </summary>
        Task<Trade> UpdateTradeAsync(Trade trade);

        /// <summary>
        /// Updates the exit information for a trade.
        /// </summary>
        Task<Trade> UpdateTradeExitAsync(Guid tradeId, DateTime exitDateTime, decimal exitPrice, string? notes);

        /// <summary>
        /// Deletes a trade by ID.
        /// </summary>
        Task<bool> DeleteTradeAsync(Guid id);

        /// <summary>
        /// Adds a management adjustment to a trade.
        /// </summary>
        Task<ManagementAdjustment> AddAdjustmentAsync(Guid tradeId, ManagementAdjustment adjustment);

        /// <summary>
        /// Retrieves all adjustments for a trade.
        /// </summary>
        Task<IEnumerable<ManagementAdjustment>> GetAdjustmentsForTradeAsync(Guid tradeId);

        /// <summary>
        /// Adds an attachment to a trade.
        /// </summary>
        Task<Attachment> AddAttachmentAsync(Guid tradeId, Attachment attachment);

        /// <summary>
        /// Retrieves all attachments for a trade.
        /// </summary>
        Task<IEnumerable<Attachment>> GetAttachmentsForTradeAsync(Guid tradeId);

        /// <summary>
        /// Deletes an attachment by ID.
        /// </summary>
        Task<bool> DeleteAttachmentAsync(Guid attachmentId);
    }
}
