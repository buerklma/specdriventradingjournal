using System;
using System.ComponentModel.DataAnnotations;

namespace TradingJournal.Data.Models
{
    public class UserPreferences
    {
        private static readonly Lazy<UserPreferences> _instance = new Lazy<UserPreferences>(() => new UserPreferences());

        public static UserPreferences Instance => _instance.Value;

        [Key]
        public Guid Id { get; set; }

        public ThemeMode Theme { get; set; }

        [Required]
        [MaxLength(3)]
        public string DefaultCurrency { get; set; } = "USD";

        public decimal DefaultRiskPercentage { get; set; } = 1.0m;

        public bool EnableBiometricAuth { get; set; }

        public string? PinHash { get; set; }

        public bool DatabaseEncrypted { get; set; }

        public string CustomFieldDefinitions { get; set; } = "{}";

        public DateTime UpdatedAt { get; set; }
    }
}
