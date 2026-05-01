using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;
namespace RentalApp.ViewModels;
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    [ObservableProperty] private ObservableCollection<Item> _items = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title = "Browse Items";
    public ItemsListViewModel(IItemRepository itemRepository) { _itemRepository = itemRepository; }
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return;
        try { IsBusy=true; HasError=false; var items=await _itemRepository.GetAllAsync(); Items.Clear(); foreach(var i in items) Items.Add(i); }
        catch(Exception ex) { ErrorMessage=$"Failed: {ex.Message}"; HasError=true; }
        finally { IsBusy=false; }
    }
    [RelayCommand]
    private async Task SearchAsync()
    {
        if (IsBusy) return;
        try { IsBusy=true; HasError=false; var items=await _itemRepository.SearchAsync(SearchText,null); Items.Clear(); foreach(var i in items) Items.Add(i); }
        catch(Exception ex) { ErrorMessage=$"Search failed: {ex.Message}"; HasError=true; }
        finally { IsBusy=false; }
    }
    [RelayCommand]
    private async Task NavigateToItemDetailAsync(Item item) { if(item is null) return; await Shell.Current.GoToAsync($"ItemDetailPage?itemId={item.Id}"); }
    [RelayCommand]
    private async Task NavigateToCreateItemAsync() => await Shell.Current.GoToAsync("CreateItemPage");
    [RelayCommand]
    private void ClearError() { HasError=false; ErrorMessage=string.Empty; }
}
