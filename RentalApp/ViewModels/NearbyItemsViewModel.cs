using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;
namespace RentalApp.ViewModels;
public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;
    [ObservableProperty] private ObservableCollection<Item> _nearbyItems=new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage=string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title="Nearby Items";
    [ObservableProperty] private double _searchRadius=5.0;
    [ObservableProperty] private string _statusMessage="Tap 'Find Near Me' to search";
    public NearbyItemsViewModel(IItemRepository ir, ILocationService ls){_itemRepository=ir;_locationService=ls;}
    [RelayCommand]
    private async Task FindNearbyAsync()
    {
        if(IsBusy) return;
        try
        {
            IsBusy=true;HasError=false;StatusMessage="Getting your location...";
            var loc=await _locationService.GetCurrentLocationAsync();
            if(!loc.HasValue){ErrorMessage="Cannot get location. Enable GPS.";HasError=true;StatusMessage="Location unavailable";return;}
            StatusMessage=$"Searching within {SearchRadius:F0}km...";
            var items=await _itemRepository.GetNearbyAsync(loc.Value.Latitude,loc.Value.Longitude,SearchRadius);
            NearbyItems.Clear();foreach(var i in items) NearbyItems.Add(i);
            StatusMessage=NearbyItems.Count>0?$"Found {NearbyItems.Count} item(s) within {SearchRadius:F0}km":$"No items found within {SearchRadius:F0}km";
        }catch(Exception ex){ErrorMessage=$"Search failed: {ex.Message}";HasError=true;StatusMessage="Search failed";}
        finally{IsBusy=false;}
    }
    [RelayCommand]
    private async Task NavigateToDetailAsync(Item item){if(item is null) return;await Shell.Current.GoToAsync($"ItemDetailPage?itemId={item.Id}");}
    [RelayCommand] private void ClearError(){HasError=false;ErrorMessage=string.Empty;}
}
