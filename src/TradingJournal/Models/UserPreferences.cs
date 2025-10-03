// <copyright file="UserPreferences.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TradingJournal.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents user preferences and settings for the application.
    /// Implements singleton pattern to ensure a single instance.
    /// </summary>
    public class UserPreferences
    {
        private static readonly Lazy<UserPreferences> LazyInstance = new Lazy<UserPreferences>(() => new UserPreferences());

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPreferences"/> class.
        /// </summary>
        public UserPreferences()
        {
            this.Id = Guid.NewGuid();
            this.Theme = ThemeMode.System;
            this.DefaultCurrency = "USD";
            this.DefaultRiskPercentage = 1m;
            this.EnableBiometricAuth = false;
            this.DatabaseEncrypted = false;
            this.CustomFieldDefinitions = "{}";
            this.UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="UserPreferences"/>.
        /// </summary>
        public static UserPreferences Instance => LazyInstance.Value;

        /// <summary>
        /// Gets or sets the unique identifier of the user preferences record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the theme mode preference.
        /// </summary>
        [Required]
        public ThemeMode Theme { get; set; }

        /// <summary>
        /// Gets or sets the default currency symbol/code (e.g., "USD").
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the default risk percentage.
        /// </summary>
        [Range(0.1, 100)]
        public decimal DefaultRiskPercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether biometric authentication is enabled.
        /// </summary>
        public bool EnableBiometricAuth { get; set; }

        /// <summary>
        /// Gets or sets the hashed PIN for authentication.
        /// </summary>
        public string? PinHash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the database is encrypted.
        /// </summary>
        public bool DatabaseEncrypted { get; set; }

        /// <summary>
        /// Gets or sets custom field definitions as JSON string.
        /// </summary>
        [Required]
        public string CustomFieldDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last update.
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
