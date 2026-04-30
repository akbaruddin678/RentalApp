using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for viewing item details and requesting rentals
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IRentalService _rentalService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private Item? _item;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _title = "Item Details";

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(3);

    [ObservableProperty]
    private string _rentalNotes = string.Empty;

    [ObservableProperty]
    private decimal _estimatedPrice;

    [ObservableProperty]
    private bool _isOwner;

    public ItemDetailViewModel(
        IItemRepository itemRepository,
        IRentalService rentalService,
        IApiService apiService)
    {
        _itemRepository = itemRepository;
        _rentalService = rentalService;
        _apiService = apiService;
    }

    partial void OnItemIdChanged(int value) => LoadItemCommand.Execute(null);

    partial void OnStartDateChanged(DateTime value) => UpdateEstimatedPrice();
    partial void OnEndDateChanged(DateTime value) => UpdateEstimatedPrice();

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        if (IsBusy || ItemId <= 0) return;
        try
        {
            IsBusy = true;
            HasError = false;
            Item = await _itemRepository.GetByIdWithDetailsAsync(ItemId);
            if (Item is not null)
            {
                Title = Item.Title;
                IsOwner = Item.OwnerId == _apiService.GetCurrentUserId();
                UpdateEstimatedPrice();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load item: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        if (Item is null || IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null)
        {
            ErrorMessage = "Please log in to request a rental.";
            HasError = true;
            return;
        }
        try
        {
            IsBusy = true;
            HasError = false;
            var (success, message, _) = await _rentalService.RequestRentalAsync(
                Item.Id, userId.Value, StartDate, EndDate, RentalNotes);

            if (success)
            {
                await Shell.Current.DisplayAlert("Success", message, "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Request failed: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task EditItemAsync()
    {
        if (Item is null) return;
        await Shell.Current.GoToAsync($"CreateItemPage?itemId={Item.Id}");
    }

    [RelayCommand]
    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private void UpdateEstimatedPrice()
    {
        if (Item is null) return;
        EstimatedPrice = _rentalService.CalculateTotalPrice(Item.DailyRate, StartDate, EndDate);
    }
}
