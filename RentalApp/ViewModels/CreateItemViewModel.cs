using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for creating or editing an item listing
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IApiService _apiService;
    private readonly ILocationService _locationService;

    [ObservableProperty] private int _itemId;
    [ObservableProperty] private string _title = "Create Listing";
    [ObservableProperty] private string _itemTitle = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _dailyRateText = string.Empty;
    [ObservableProperty] private string _locationName = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private int _selectedCategoryIndex;
    [ObservableProperty] private bool _isEditMode;

    public ObservableCollection<string> Categories { get; } = new()
    {
        "Tools", "Camping Gear", "Board Games", "Sports", "Electronics", "Garden"
    };

    private double? _latitude;
    private double? _longitude;

    public CreateItemViewModel(
        IItemRepository itemRepository,
        IApiService apiService,
        ILocationService locationService)
    {
        _itemRepository = itemRepository;
        _apiService = apiService;
        _locationService = locationService;
    }

    partial void OnItemIdChanged(int value)
    {
        if (value > 0) LoadItemCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        if (ItemId <= 0) return;
        try
        {
            IsBusy = true;
            var item = await _itemRepository.GetByIdAsync(ItemId);
            if (item is not null)
            {
                IsEditMode = true;
                Title = "Edit Listing";
                ItemTitle = item.Title;
                Description = item.Description;
                DailyRateText = item.DailyRate.ToString("F2");
                LocationName = item.LocationName;
                _latitude = item.Latitude;
                _longitude = item.Longitude;
                SelectedCategoryIndex = Math.Max(0, (item.CategoryId ?? 1) - 1);
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
    private async Task GetLocationAsync()
    {
        try
        {
            IsBusy = true;
            var location = await _locationService.GetCurrentLocationAsync();
            if (location.HasValue)
            {
                _latitude = location.Value.Latitude;
                _longitude = location.Value.Longitude;
                LocationName = $"{location.Value.Latitude:F4}, {location.Value.Longitude:F4}";
            }
            else
            {
                ErrorMessage = "Could not get location. Please check permissions.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Location error: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SaveItemAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(ItemTitle))
        {
            ErrorMessage = "Please enter a title.";
            HasError = true;
            return;
        }
        if (!decimal.TryParse(DailyRateText, out var dailyRate) || dailyRate <= 0)
        {
            ErrorMessage = "Please enter a valid daily rate.";
            HasError = true;
            return;
        }
        var userId = _apiService.GetCurrentUserId();
        if (userId is null)
        {
            ErrorMessage = "Please log in first.";
            HasError = true;
            return;
        }
        try
        {
            IsBusy = true;
            HasError = false;
            if (IsEditMode)
            {
                var existing = await _itemRepository.GetByIdAsync(ItemId);
                if (existing is not null)
                {
                    existing.Title = ItemTitle;
                    existing.Description = Description;
                    existing.DailyRate = dailyRate;
                    existing.LocationName = LocationName;
                    existing.Latitude = _latitude;
                    existing.Longitude = _longitude;
                    existing.CategoryId = SelectedCategoryIndex + 1;
                    await _itemRepository.UpdateAsync(existing);
                }
            }
            else
            {
                var item = new Item
                {
                    Title = ItemTitle,
                    Description = Description,
                    DailyRate = dailyRate,
                    LocationName = LocationName,
                    Latitude = _latitude,
                    Longitude = _longitude,
                    CategoryId = SelectedCategoryIndex + 1,
                    OwnerId = userId.Value,
                    IsAvailable = true
                };
                await _itemRepository.AddAsync(item);
            }
            await Shell.Current.DisplayAlert(
                "Success", IsEditMode ? "Item updated!" : "Item listed!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Save failed: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
