using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
namespace RentalApp.ViewModels;
[QueryProperty(nameof(ItemId),"itemId")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;
    private readonly IApiService _apiService;
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private Item? _item;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage=string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title="Item Details";
    [ObservableProperty] private DateTime _startDate=DateTime.Today.AddDays(1);
    [ObservableProperty] private DateTime _endDate=DateTime.Today.AddDays(3);
    [ObservableProperty] private string _rentalNotes=string.Empty;
    [ObservableProperty] private decimal _estimatedPrice;
    [ObservableProperty] private bool _isOwner;
    public ItemDetailViewModel(IItemRepository ir, IRentalRepository rr, IRentalService rs, IApiService api)
    { _itemRepository=ir; _rentalRepository=rr; _rentalService=rs; _apiService=api; }
    partial void OnItemIdChanged(int value) => LoadItemCommand.Execute(null);
    partial void OnStartDateChanged(DateTime v) => UpdatePrice();
    partial void OnEndDateChanged(DateTime v) => UpdatePrice();
    [RelayCommand]
    private async Task LoadItemAsync()
    {
        if(IsBusy||ItemId<=0) return;
        try { IsBusy=true; Item=await _itemRepository.GetByIdAsync(ItemId); if(Item is not null){Title=Item.Title;IsOwner=Item.OwnerId==_apiService.GetCurrentUserId();UpdatePrice();} }
        catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}
        finally{IsBusy=false;}
    }
    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        if(Item is null||IsBusy) return;
        var userId=_apiService.GetCurrentUserId();
        if(userId is null){ErrorMessage="Please log in first.";HasError=true;return;}
        var (valid,msg)=_rentalService.ValidateDates(StartDate,EndDate);
        if(!valid){ErrorMessage=msg;HasError=true;return;}
        try
        {
            IsBusy=true;
            var canBook=await _rentalService.CanRequestRentalAsync(Item.Id,StartDate,EndDate);
            if(!canBook){ErrorMessage="Item already booked for these dates.";HasError=true;return;}
            var rental=new Rental{ItemId=Item.Id,BorrowerId=userId.Value,StartDate=StartDate,EndDate=EndDate,
                TotalPrice=_rentalService.CalculateTotalPrice(Item.DailyRate,StartDate,EndDate),Notes=RentalNotes,Status=RentalStatus.Requested};
            await _rentalRepository.CreateAsync(rental);
            await Shell.Current.DisplayAlertAsync("Success","Rental requested!","OK");
            await Shell.Current.GoToAsync("..");
        }
        catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}
        finally{IsBusy=false;}
    }
    [RelayCommand]
    private async Task EditItemAsync() { if(Item is null) return; await Shell.Current.GoToAsync($"CreateItemPage?itemId={Item.Id}"); }
    [RelayCommand] private void ClearError(){HasError=false;ErrorMessage=string.Empty;}
    private void UpdatePrice(){if(Item is null) return; EstimatedPrice=_rentalService.CalculateTotalPrice(Item.DailyRate,StartDate,EndDate);}
}
