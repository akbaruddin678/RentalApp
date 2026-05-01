using RentalApp.Database.Models;
namespace RentalApp.Services;
public interface IReviewService
{
    bool CanReview(RentalStatus rentalStatus);
    double CalculateAverageRating(IEnumerable<Review> reviews);
}
