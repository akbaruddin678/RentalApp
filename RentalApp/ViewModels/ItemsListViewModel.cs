using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for browsing all available items
/// </summary>
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _title = "Browse Items";

    public ItemsListViewModel(IItemRepository itemRepository, IServiceProvider serviceProvider)
    {
        _itemRepository = itemRepository;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;
            var items = await _itemRepository.GetAllWithDetailsAsync();
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load items: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;
            var items = string.IsNullOrWhiteSpace(SearchText)
                ? await _itemRepository.GetAllWithDetailsAsync()
                : await _itemRepository.SearchAsync(SearchText);
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Search failed: {ex.Message}";
            HasError = true;
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
    private async Task NavigateToCreateItemAsync()
    {
        await Shell.Current.GoToAsync("CreateItemPage");
    }

    [RelayCommand]
    private void ClearError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }
}
