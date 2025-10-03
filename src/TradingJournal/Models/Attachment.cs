// <copyright file="Attachment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a file attached to a trade, such as screenshots or documents.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Gets or sets the unique identifier of the attachment.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated trade.
        /// </summary>
        [Required]
        public Guid TradeId { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the MIME type or file type.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FileType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the size of the file in bytes.
        /// </summary>
        [Required]
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the storage path of the file.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string StoragePath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the caption or description of the attachment.
        /// </summary>
        [MaxLength(500)]
        public string? Caption { get; set; }

        /// <summary>
        /// Gets or sets the upload timestamp.
        /// </summary>
        [Required]
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the related trade.
        /// </summary>
        public Trade Trade { get; set; } = null!;
    }
}
