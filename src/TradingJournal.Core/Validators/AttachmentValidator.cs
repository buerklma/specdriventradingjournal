using FluentValidation;
using System.Linq;
using TradingJournal.Data.Models;

namespace TradingJournal.Core.Validators
{
    /// <summary>
    /// Validates <see cref="Attachment"/> entities.
    /// </summary>
    public class AttachmentValidator : AbstractValidator<Attachment>
    {
        public AttachmentValidator()
        {
            RuleFor(x => x.FileSizeBytes)
                .LessThanOrEqualTo(10 * 1024 * 1024)
                .WithMessage("File size must be 10MB or less.");

            RuleFor(x => x.FileName)
                .NotEmpty();

            RuleFor(x => x.FileType)
                .Must(type => new[] {".png", ".jpg", ".pdf", ".txt"}.Contains(type.ToLower()))
                .WithMessage("Unsupported file type.");
        }
    }
}
