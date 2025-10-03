using System;
using System.ComponentModel.DataAnnotations;

namespace TradingJournal.Data.Models
{
    /// <summary>
    /// Represents a file attachment related to a trade.
    /// </summary>
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TradeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FileType { get; set; } = null!;

        /// <summary>Gets or sets the size of the file in bytes.</summary>
        [Required]
        public long FileSizeBytes { get; set; }

        /// <summary>Gets or sets the storage path of the file.</summary>
        [Required]
        [MaxLength(500)]
        public string StoragePath { get; set; } = null!;

        /// <summary>Gets or sets the caption or description of the attachment.</summary>
        [MaxLength(500)]
        public string? Caption { get; set; }

        /// <summary>Gets or sets the upload timestamp.</summary>
        [Required]
        public DateTime UploadedAt { get; set; }

        public Trade Trade { get; set; } = null!;
    }
}
