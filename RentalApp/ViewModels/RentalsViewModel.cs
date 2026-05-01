using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;
namespace RentalApp.ViewModels;
public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;
    private readonly IApiService _apiService;
    [ObservableProperty] private ObservableCollection<Rental> _incomingRentals=new();
    [ObservableProperty] private ObservableCollection<Rental> _outgoingRentals=new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage=string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title="My Rentals";
    public RentalsViewModel(IRentalRepository rr, IRentalService rs, IApiService api){_rentalRepository=rr;_rentalService=rs;_apiService=api;}
    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if(IsBusy) return;var userId=_apiService.GetCurrentUserId();if(userId is null) return;
        try{IsBusy=true;HasError=false;await _rentalService.DetectOverdueAsync();
            var inc=await _rentalRepository.GetIncomingAsync(userId.Value);IncomingRentals.Clear();foreach(var r in inc) IncomingRentals.Add(r);
            var out2=await _rentalRepository.GetOutgoingAsync(userId.Value);OutgoingRentals.Clear();foreach(var r in out2) OutgoingRentals.Add(r);
        }catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}finally{IsBusy=false;}
    }
    [RelayCommand] private async Task ApproveAsync(Rental r)=>await TransitionAsync(r,RentalStatus.Approved);
    [RelayCommand] private async Task RejectAsync(Rental r)=>await TransitionAsync(r,RentalStatus.Rejected);
    [RelayCommand] private async Task StartAsync(Rental r)=>await TransitionAsync(r,RentalStatus.OutForRent);
    [RelayCommand] private async Task ReturnAsync(Rental r)=>await TransitionAsync(r,RentalStatus.Returned);
    [RelayCommand] private async Task CompleteAsync(Rental r)=>await TransitionAsync(r,RentalStatus.Completed);
    private async Task TransitionAsync(Rental rental, RentalStatus newStatus)
    {
        if(rental is null||IsBusy) return;
        if(!_rentalService.CanTransitionTo(rental.Status,newStatus)){ErrorMessage=$"Cannot change from {rental.Status} to {newStatus}.";HasError=true;return;}
        try{IsBusy=true;await _rentalRepository.UpdateStatusAsync(rental.Id,newStatus);await LoadRentalsAsync();}
        catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}finally{IsBusy=false;}
    }
    [RelayCommand] private void ClearError(){HasError=false;ErrorMessage=string.Empty;}
}
