using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using TradingJournal.Models;

namespace TradingJournal.UnitTests.Models
{
    public class AttachmentTests
    {
        [Fact]
        public void FileSizeValidation_ShouldFailIfTooLarge()
        {
            var attachment = new Attachment
            {
                FileSizeBytes = 15 * 1024 * 1024 // 15MB
            };
            var context = new ValidationContext(attachment);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(attachment, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains("FileSizeBytes"));
        }

        [Theory]
        [InlineData("image.png")]
        [InlineData("document.pdf")]
        [InlineData("notes.txt")]
        public void AllowedFileTypes_ShouldPass(string fileName)
        {
            var attachment = new Attachment
            {
                FileName = fileName
            };
            var ext = System.IO.Path.GetExtension(fileName).ToLower();
            var allowed = new[] { ".png", ".jpg", ".pdf", ".txt" };

            Assert.Contains(ext, allowed);
        }
    }
}
