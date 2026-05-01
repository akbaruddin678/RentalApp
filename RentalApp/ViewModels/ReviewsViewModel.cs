using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using System.Collections.ObjectModel;
namespace RentalApp.ViewModels;
[QueryProperty(nameof(ItemId),"itemId")]
[QueryProperty(nameof(RentalId),"rentalId")]
public partial class ReviewsViewModel : ObservableObject
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewService _reviewService;
    private readonly IApiService _apiService;
    [ObservableProperty] private int _itemId;
    [ObservableProperty] private int _rentalId;
    [ObservableProperty] private ObservableCollection<Review> _reviews=new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage=string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private string _successMessage=string.Empty;
    [ObservableProperty] private bool _hasSuccess;
    [ObservableProperty] private string _title="Reviews";
    [ObservableProperty] private int _newRating=5;
    [ObservableProperty] private string _newComment=string.Empty;
    [ObservableProperty] private double _averageRating;
    [ObservableProperty] private bool _canReview;
    public ReviewsViewModel(IReviewRepository rr, IReviewService rs, IApiService api){_reviewRepository=rr;_reviewService=rs;_apiService=api;}
    partial void OnItemIdChanged(int value){if(value>0) LoadReviewsCommand.Execute(null);}
    partial void OnRentalIdChanged(int value){CanReview=value>0;}
    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        if(IsBusy||ItemId<=0) return;
        try{IsBusy=true;HasError=false;var reviews=await _reviewRepository.GetByItemAsync(ItemId);
            Reviews.Clear();foreach(var r in reviews) Reviews.Add(r);AverageRating=_reviewService.CalculateAverageRating(reviews);}
        catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}finally{IsBusy=false;}
    }
    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if(IsBusy||RentalId<=0) return;
        var userId=_apiService.GetCurrentUserId();
        if(userId is null){ErrorMessage="Please log in first.";HasError=true;return;}
        if(NewRating<1||NewRating>5){ErrorMessage="Rating must be 1-5.";HasError=true;return;}
        try
        {
            IsBusy=true;HasError=false;
            if(await _reviewRepository.ExistsForRentalAsync(RentalId)){ErrorMessage="Already reviewed.";HasError=true;return;}
            var review=new Review{RentalId=RentalId,ItemId=ItemId,ReviewerId=userId.Value,Rating=NewRating,Comment=NewComment};
            await _reviewRepository.CreateAsync(review);
            SuccessMessage="Review submitted!";HasSuccess=true;CanReview=false;NewComment=string.Empty;NewRating=5;
            await LoadReviewsAsync();
        }catch(Exception ex){ErrorMessage=ex.Message;HasError=true;}finally{IsBusy=false;}
    }
    [RelayCommand] private void ClearError(){HasError=false;ErrorMessage=string.Empty;}
}
