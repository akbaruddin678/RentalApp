using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for location-based item discovery using PostGIS
/// </summary>
public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;

    [ObservableProperty] private ObservableCollection<Item> _nearbyItems = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title = "Nearby Items";
    [ObservableProperty] private double _searchRadius = 5.0;
    [ObservableProperty] private string _locationStatus = "Tap 'Find Near Me' to search";
    [ObservableProperty] private bool _hasResults;

    public NearbyItemsViewModel(
        IItemRepository itemRepository,
        ILocationService locationService)
    {
        _itemRepository = itemRepository;
        _locationService = locationService;
    }

    [RelayCommand]
    private async Task FindNearbyAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;
            LocationStatus = "Getting your location...";

            var location = await _locationService.GetCurrentLocationAsync();
            if (!location.HasValue)
            {
                ErrorMessage = "Could not get your location. Please enable GPS.";
                HasError = true;
                LocationStatus = "Location unavailable";
                return;
            }

            LocationStatus = $"Searching within {SearchRadius}km...";
            var items = await _itemRepository.GetNearbyItemsAsync(
                location.Value.Latitude,
                location.Value.Longitude,
                SearchRadius);

            NearbyItems.Clear();
            foreach (var item in items)
                NearbyItems.Add(item);

            HasResults = NearbyItems.Any();
            LocationStatus = HasResults
                ? $"Found {NearbyItems.Count} item(s) within {SearchRadius}km"
                : $"No items found within {SearchRadius}km";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Search failed: {ex.Message}";
            HasError = true;
            LocationStatus = "Search failed";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task NavigateToItemDetailAsync(Item item)
    {
        if (item is null) return;
        await Shell.Current.GoToAsync($"ItemDetailPage?itemId={item.Id}");
    }

    [RelayCommand]
    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
