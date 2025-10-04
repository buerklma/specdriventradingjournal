using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingJournal.Core.Interfaces;
using TradingJournal.Core.Models;
using TradingJournal.Data;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Services
{
    /// <summary>
    /// Service for managing trade operations including CRUD, filtering, and related entities.
    /// </summary>
    public class TradeService : ITradeService
    {
        private readonly TradingDbContext _context;

        public TradeService(TradingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Trade> CreateTradeAsync(Trade trade)
        {
            if (trade == null)
            {
                throw new ArgumentNullException(nameof(trade));
            }

            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
            return trade;
        }

        public async Task<Trade?> GetTradeByIdAsync(Guid id)
        {
            return await _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Trade>> GetAllTradesAsync()
        {
            return await _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .OrderByDescending(t => t.EntryDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trade>> GetTradesBySymbolAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));
            }

            return await _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .Where(t => t.Symbol == symbol)
                .OrderByDescending(t => t.EntryDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trade>> GetTradesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .Where(t => t.EntryDateTime >= startDate && t.EntryDateTime <= endDate)
                .OrderByDescending(t => t.EntryDateTime)
                .ToListAsync();
        }

        public async Task<PagedResult<Trade>> GetTradesPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException("Page number must be greater than 0.", nameof(pageNumber));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));
            }

            var totalCount = await _context.Trades.CountAsync();
            var trades = await _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .OrderByDescending(t => t.EntryDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Trade>
            {
                Items = trades,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<Trade>> GetTradesPagedAsync(int pageNumber, int pageSize, TradeFilter filter)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException("Page number must be greater than 0.", nameof(pageNumber));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var query = _context.Trades
                .AsNoTracking()
                .Include(t => t.Adjustments)
                .Include(t => t.Attachments)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.Symbol))
            {
                query = query.Where(t => t.Symbol == filter.Symbol);
            }

            if (!string.IsNullOrWhiteSpace(filter.SetupType))
            {
                query = query.Where(t => t.SetupType == filter.SetupType);
            }

            if (filter.Direction.HasValue)
            {
                query = query.Where(t => t.Direction == filter.Direction.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(t => t.EntryDateTime >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(t => t.EntryDateTime <= filter.EndDate.Value);
            }

            if (filter.MinProfitLoss.HasValue)
            {
                query = query.Where(t => t.ProfitLossCurrency >= filter.MinProfitLoss.Value);
            }

            if (filter.MaxProfitLoss.HasValue)
            {
                query = query.Where(t => t.ProfitLossCurrency <= filter.MaxProfitLoss.Value);
            }

            if (filter.MinRRRatio.HasValue)
            {
                query = query.Where(t => t.RealizedRRRatio >= filter.MinRRRatio.Value);
            }

            if (filter.MaxRRRatio.HasValue)
            {
                query = query.Where(t => t.RealizedRRRatio <= filter.MaxRRRatio.Value);
            }

            var totalCount = await query.CountAsync();
            var trades = await query
                .OrderByDescending(t => t.EntryDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Trade>
            {
                Items = trades,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Trade> UpdateTradeAsync(Trade trade)
        {
            if (trade == null)
            {
                throw new ArgumentNullException(nameof(trade));
            }

            var existingTrade = await _context.Trades.FindAsync(trade.Id);
            if (existingTrade == null)
            {
                throw new InvalidOperationException($"Trade with ID {trade.Id} not found.");
            }

            _context.Entry(existingTrade).CurrentValues.SetValues(trade);
            await _context.SaveChangesAsync();
            return existingTrade;
        }

        public async Task<Trade> UpdateTradeExitAsync(Guid tradeId, DateTime exitDateTime, decimal exitPrice, string? notes)
        {
            var trade = await _context.Trades.FindAsync(tradeId);
            if (trade == null)
            {
                throw new InvalidOperationException($"Trade with ID {tradeId} not found.");
            }

            trade.ExitDateTime = exitDateTime;
            trade.ExitPrice = exitPrice;
            if (!string.IsNullOrEmpty(notes))
            {
                trade.Notes = trade.Notes + "\n" + notes;
            }

            // Recalculate P&L and R-multiple based on exit price
            if (trade.Direction == TradeDirection.Long)
            {
                trade.ProfitLossCurrency = (exitPrice - trade.EntryPrice) * trade.PositionSize;
            }
            else
            {
                trade.ProfitLossCurrency = (trade.EntryPrice - exitPrice) * trade.PositionSize;
            }

            var riskAmount = Math.Abs(trade.EntryPrice - trade.StopLoss) * trade.PositionSize;
            if (riskAmount > 0 && trade.ProfitLossCurrency.HasValue)
            {
                trade.RealizedRRRatio = trade.ProfitLossCurrency.Value / riskAmount;
            }

            trade.HoldingTime = exitDateTime - trade.EntryDateTime;

            trade.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return trade;
        }

        public async Task<bool> DeleteTradeAsync(Guid id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade == null)
            {
                return false;
            }

            _context.Trades.Remove(trade);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ManagementAdjustment> AddAdjustmentAsync(Guid tradeId, ManagementAdjustment adjustment)
        {
            if (adjustment == null)
            {
                throw new ArgumentNullException(nameof(adjustment));
            }

            var trade = await _context.Trades.FindAsync(tradeId);
            if (trade == null)
            {
                throw new InvalidOperationException($"Trade with ID {tradeId} not found.");
            }

            adjustment.TradeId = tradeId;
            _context.ManagementAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();
            return adjustment;
        }

        public async Task<IEnumerable<ManagementAdjustment>> GetAdjustmentsForTradeAsync(Guid tradeId)
        {
            return await _context.ManagementAdjustments
                .Where(a => a.TradeId == tradeId)
                .OrderBy(a => a.AdjustmentDateTime)
                .ToListAsync();
        }

        public async Task<Attachment> AddAttachmentAsync(Guid tradeId, Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            var trade = await _context.Trades.FindAsync(tradeId);
            if (trade == null)
            {
                throw new InvalidOperationException($"Trade with ID {tradeId} not found.");
            }

            attachment.TradeId = tradeId;
            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsForTradeAsync(Guid tradeId)
        {
            return await _context.Attachments
                .Where(a => a.TradeId == tradeId)
                .OrderBy(a => a.UploadedAt)
                .ToListAsync();
        }

        public async Task<bool> DeleteAttachmentAsync(Guid attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            if (attachment == null)
            {
                return false;
            }

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
