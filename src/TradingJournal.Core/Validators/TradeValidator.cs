using FluentValidation;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Validators
{
    /// <summary>
    /// Validates <see cref="Trade"/> entities.
    /// </summary>
    public class TradeValidator : AbstractValidator<Trade>
    {
        public TradeValidator()
        {
            RuleFor(x => x.Symbol)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.EntryDateTime)
                .NotEmpty();

            RuleFor(x => x.EntryPrice)
                .GreaterThan(0);

            RuleFor(x => x.StopLoss)
                .GreaterThan(0);

            RuleFor(x => x.TakeProfit)
                .GreaterThan(0);

            RuleFor(x => x.RiskPercentage)
                .InclusiveBetween(0.1m, 100m);

            RuleFor(x => x)
                .Must(x => x.Direction == TradeDirection.Long ? x.StopLoss < x.EntryPrice && x.EntryPrice < x.TakeProfit : x.TakeProfit < x.EntryPrice && x.EntryPrice < x.StopLoss)
                .WithMessage("StopLoss, EntryPrice, and TakeProfit must follow direction-specific order.");
        }
    }
}
