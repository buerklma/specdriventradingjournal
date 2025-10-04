using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for managing trade attachments (screenshots, charts).
    /// </summary>
    public partial class AttachmentsViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private Guid tradeId;

        [ObservableProperty]
        private ObservableCollection<Attachment> attachments = new();

        public AttachmentsViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Attachments";
        }

        public async Task LoadAttachmentsAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            TradeId = id;

            try
            {
                var attachmentList = await _tradeService.GetAttachmentsForTradeAsync(id);

                Attachments.Clear();
                foreach (var attachment in attachmentList)
                {
                    Attachments.Add(attachment);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load attachments: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task AddAttachmentAsync()
        {
            if (IsBusy || TradeId == Guid.Empty)
            {
                return;
            }

            try
            {
                IsBusy = true;

                // In a real implementation, this would open a file picker
                await Shell.Current.DisplayAlert(
                    "Add Attachment",
                    "File picker functionality needs to be implemented. This will allow selecting screenshots or charts to attach.",
                    "OK");

                // Example implementation with FilePicker:
                // var result = await FilePicker.PickAsync(new PickOptions
                // {
                //     PickerTitle = "Select File to Attach",
                //     FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                //     {
                //         { DevicePlatform.WinUI, new[] { ".png", ".jpg", ".jpeg", ".pdf", ".txt" } }
                //     })
                // });
                //
                // if (result != null)
                // {
                //     var attachment = new Attachment
                //     {
                //         Id = Guid.NewGuid(),
                //         TradeId = TradeId,
                //         FileName = result.FileName,
                //         FileType = Path.GetExtension(result.FileName),
                //         FileSizeBytes = new FileInfo(result.FullPath).Length,
                //         StoragePath = result.FullPath,
                //         UploadedAt = DateTime.UtcNow
                //     };
                //
                //     await _tradeService.AddAttachmentAsync(TradeId, attachment);
                //     await LoadAttachmentsAsync(TradeId);
                // }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to add attachment: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            if (IsBusy || attachmentId == Guid.Empty)
            {
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Delete Attachment",
                "Are you sure you want to delete this attachment?",
                "Yes",
                "No");

            if (!confirm)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var success = await _tradeService.DeleteAttachmentAsync(attachmentId);

                if (success)
                {
                    await LoadAttachmentsAsync(TradeId);
                    await Shell.Current.DisplayAlert("Success", "Attachment deleted successfully", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to delete attachment", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to delete attachment: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
