using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TradingJournal.Core.Interfaces;
using TradingJournal.Core.Models;
using TradingJournal.Data.Models;

namespace TradingJournal.ViewModels
{
    /// <summary>
    /// ViewModel for displaying and filtering the list of trades.
    /// </summary>
    public partial class TradesListViewModel : BaseViewModel
    {
        private readonly ITradeService _tradeService;

        [ObservableProperty]
        private ObservableCollection<Trade> trades = new();

        [ObservableProperty]
        private TradeFilter? currentFilter;

        [ObservableProperty]
        private int pageNumber = 1;

        [ObservableProperty]
        private int pageSize = 50;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private bool hasNextPage;

        [ObservableProperty]
        private bool hasPreviousPage;

        public TradesListViewModel(ITradeService tradeService)
        {
            _tradeService = tradeService;
            Title = "Trades";
        }

        [RelayCommand]
        private async Task LoadTradesPagedAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                PagedResult<Trade> result;
                if (CurrentFilter != null)
                {
                    result = await _tradeService.GetTradesPagedAsync(PageNumber, PageSize, CurrentFilter);
                }
                else
                {
                    result = await _tradeService.GetTradesPagedAsync(PageNumber, PageSize);
                }

                Trades.Clear();
                foreach (var trade in result.Items)
                {
                    Trades.Add(trade);
                }

                TotalPages = (int)Math.Ceiling((double)result.TotalCount / PageSize);
                HasNextPage = PageNumber < TotalPages;
                HasPreviousPage = PageNumber > 1;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Error",
                    $"Failed to load trades: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToTradeDetailAsync(Guid tradeId)
        {
            if (tradeId == Guid.Empty)
            {
                return;
            }

            await Shell.Current.GoToAsync($"TradeDetail?Id={tradeId}");
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (HasNextPage)
            {
                PageNumber++;
                await LoadTradesPagedAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (HasPreviousPage)
            {
                PageNumber--;
                await LoadTradesPagedAsync();
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            PageNumber = 1;
            await LoadTradesPagedAsync();
        }
    }
}
