using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;

namespace RentalApp.ViewModels;

/// <summary>
/// ViewModel for submitting and viewing reviews
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
[QueryProperty(nameof(RentalId), "rentalId")]
public partial class ReviewsViewModel : ObservableObject
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewService _reviewService;
    private readonly IApiService _apiService;

    [ObservableProperty] private int _itemId;
    [ObservableProperty] private int _rentalId;
    [ObservableProperty] private ObservableCollection<Review> _reviews = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _title = "Reviews";
    [ObservableProperty] private int _newRating = 5;
    [ObservableProperty] private string _newComment = string.Empty;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private bool _canReview;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private bool _hasSuccess;

    public ReviewsViewModel(
        IReviewRepository reviewRepository,
        IReviewService reviewService,
        IApiService apiService)
    {
        _reviewRepository = reviewRepository;
        _reviewService = reviewService;
        _apiService = apiService;
    }

    partial void OnItemIdChanged(int value)
    {
        if (value > 0) LoadReviewsCommand.Execute(null);
    }

    partial void OnRentalIdChanged(int value)
    {
        CanReview = value > 0;
    }

    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        if (IsBusy || ItemId <= 0) return;
        try
        {
            IsBusy = true;
            HasError = false;
            var reviews = await _reviewRepository.GetByItemIdAsync(ItemId);
            Reviews.Clear();
            foreach (var r in reviews) Reviews.Add(r);
            AverageRating = await _reviewService.GetAverageRatingAsync(ItemId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load reviews: {ex.Message}";
            HasError = true;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if (IsBusy || RentalId <= 0) return;
        var userId = _apiService.GetCurrentUserId();
        if (userId is null)
        {
            ErrorMessage = "Please log in to submit a review.";
            HasError = true;
            return;
        }
        try
        {
            IsBusy = true;
            HasError = false;
            var (success, message, _) = await _reviewService.SubmitReviewAsync(
                RentalId, userId.Value, NewRating, NewComment);

            if (success)
            {
                SuccessMessage = "Review submitted!";
                HasSuccess = true;
                NewComment = string.Empty;
                NewRating = 5;
                CanReview = false;
                await LoadReviewsAsync();
            }
            else
            {
                ErrorMessage = message;
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Submit failed: {ex.Message}";
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
