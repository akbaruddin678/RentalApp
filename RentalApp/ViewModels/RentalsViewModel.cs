using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for managing incoming and outgoing rentals
/// </summary>
public partial class RentalsViewModel : ObservableObject
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IRentalService _rentalService;
    private readonly IApiService _apiService;

    [ObservableProperty] private ObservableCollection<Rental> _incomingRentals = new();
    [ObservableProperty] private ObservableCollection<Rental> _outgoingRentals = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title = "My Rentals";
    [ObservableProperty] private int _selectedTabIndex;

    public RentalsViewModel(
        IRentalRepository rentalRepository,
        IRentalService rentalService,
        IApiService apiService)
    {
        _rentalRepository = rentalRepository;
        _rentalService = rentalService;
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null) return;
        try
        {
            IsBusy = true;
            HasError = false;

            var incoming = await _rentalRepository.GetIncomingRentalsAsync(userId.Value);
            IncomingRentals.Clear();
            foreach (var r in incoming) IncomingRentals.Add(r);

            var outgoing = await _rentalRepository.GetOutgoingRentalsAsync(userId.Value);
            OutgoingRentals.Clear();
            foreach (var r in outgoing) OutgoingRentals.Add(r);

            // Auto detect overdue
            await _rentalService.DetectOverdueRentalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load rentals: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ApproveRentalAsync(Rental rental)
    {
        if (rental is null || IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null) return;
        try
        {
            IsBusy = true;
            var (success, message) = await _rentalService.ApproveRentalAsync(rental.Id, userId.Value);
            if (success)
                await LoadRentalsAsync();
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Approve failed: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task RejectRentalAsync(Rental rental)
    {
        if (rental is null || IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null) return;
        try
        {
            IsBusy = true;
            var (success, message) = await _rentalService.RejectRentalAsync(rental.Id, userId.Value);
            if (success)
                await LoadRentalsAsync();
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Reject failed: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task StartRentalAsync(Rental rental)
    {
        if (rental is null || IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null) return;
        try
        {
            IsBusy = true;
            var (success, message) = await _rentalService.StartRentalAsync(rental.Id, userId.Value);
            if (success)
                await LoadRentalsAsync();
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Start failed: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ReturnRentalAsync(Rental rental)
    {
        if (rental is null || IsBusy) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null) return;
        try
        {
            IsBusy = true;
            var (success, message) = await _rentalService.ReturnRentalAsync(rental.Id, userId.Value);
            if (success)
                await LoadRentalsAsync();
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Return failed: {ex.Message}";
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
