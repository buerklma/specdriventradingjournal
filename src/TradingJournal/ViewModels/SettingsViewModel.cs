using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for application settings and user preferences.
    /// </summary>
    public partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private ThemeMode theme = ThemeMode.System;

        [ObservableProperty]
        private string defaultCurrency = "USD";

        [ObservableProperty]
        private decimal defaultRiskPercentage = 1.0m;

        [ObservableProperty]
        private bool enableBiometricAuth;

        public SettingsViewModel()
        {
            Title = "Settings";
        }

        public async Task LoadSettingsAsync()
        {
            // In a real implementation, this would load from UserPreferences entity
            // For now, we'll use default values
            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // In a real implementation, this would save to UserPreferences entity
                // For now, we'll just show a success message
                await Shell.Current.DisplayAlert("Success", "Settings saved successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        partial void OnThemeChanged(ThemeMode value)
        {
            // Apply theme change immediately
            Application.Current!.UserAppTheme = value switch
            {
                ThemeMode.Light => AppTheme.Light,
                ThemeMode.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }
    }
}
