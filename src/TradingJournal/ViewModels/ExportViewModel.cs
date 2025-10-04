using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for exporting data and managing backups.
    /// </summary>
    public partial class ExportViewModel : BaseViewModel
    {
        private readonly IExportService _exportService;

        [ObservableProperty]
        private string? lastExportPath;

        [ObservableProperty]
        private string? lastBackupPath;

        public ExportViewModel(IExportService exportService)
        {
            _exportService = exportService;
            Title = "Export & Backup";
        }

        [RelayCommand]
        private async Task ExportToCsvAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"TradingJournal_Export_{DateTime.Now:yyyy-MM-dd_HHmmss}.csv";
                var outputPath = Path.Combine(documentsPath, "TradingJournal", "Exports", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                LastExportPath = await _exportService.ExportToCsvAsync(outputPath);

                await Shell.Current.DisplayAlert("Success", $"Exported to: {LastExportPath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to export: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ExportToExcelAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"TradingJournal_Export_{DateTime.Now:yyyy-MM-dd_HHmmss}.xlsx";
                var outputPath = Path.Combine(documentsPath, "TradingJournal", "Exports", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                LastExportPath = await _exportService.ExportToExcelAsync(outputPath);

                await Shell.Current.DisplayAlert("Success", $"Exported to: {LastExportPath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to export: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ExportToPdfAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"TradingJournal_Report_{DateTime.Now:yyyy-MM-dd_HHmmss}.pdf";
                var outputPath = Path.Combine(documentsPath, "TradingJournal", "Reports", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                LastExportPath = await _exportService.ExportToPdfAsync(outputPath);

                await Shell.Current.DisplayAlert("Success", $"Report generated: {LastExportPath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to generate report: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreateBackupAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"TradingJournal_Backup_{DateTime.Now:yyyy-MM-dd_HHmmss}.zip";
                var outputPath = Path.Combine(documentsPath, "TradingJournal", "Backups", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                LastBackupPath = await _exportService.CreateBackupAsync(outputPath);

                await Shell.Current.DisplayAlert("Success", $"Backup created: {LastBackupPath}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to create backup: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RestoreBackupAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // In a real implementation, this would open a file picker
                // For now, we'll show a message
                await Shell.Current.DisplayAlert(
                    "Restore Backup",
                    "Please select a backup file to restore. This feature requires file picker implementation.",
                    "OK");

                // Example implementation:
                // var result = await FilePicker.PickAsync(new PickOptions
                // {
                //     PickerTitle = "Select Backup File",
                //     FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                //     {
                //         { DevicePlatform.WinUI, new[] { ".zip" } }
                //     })
                // });
                //
                // if (result != null)
                // {
                //     await _exportService.RestoreFromBackupAsync(result.FullPath, overwriteExisting: true);
                //     await Shell.Current.DisplayAlert("Success", "Backup restored successfully", "OK");
                // }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to restore backup: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
