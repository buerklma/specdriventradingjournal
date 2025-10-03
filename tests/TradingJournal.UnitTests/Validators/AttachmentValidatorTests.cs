using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Data.Models;
using TradingJournal.Core.Validators;

namespace TradingJournal.UnitTests.Validators
{
    public class AttachmentValidatorTests
    {
        [Fact]
        public void FileSizeTooLarge_ShouldFailValidation()
        {
            var attachment = new Attachment { FileSizeBytes = 20L * 1024 * 1024 }; // 20MB
            var validator = new AttachmentValidator();
            var result = validator.Validate(attachment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FileSizeBytes");
        }

        [Fact]
        public void InvalidFileType_ShouldFailValidation()
        {
            var attachment = new Attachment { FileName = "malware.exe" };
            var validator = new AttachmentValidator();
            var result = validator.Validate(attachment);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FileType");
        }
    }
}
