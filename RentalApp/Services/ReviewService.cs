using RentalApp.Database.Models;
namespace RentalApp.Services;
public class ReviewService : IReviewService
{
    public bool CanReview(RentalStatus status)
        => status is RentalStatus.Returned or RentalStatus.Completed;
    public double CalculateAverageRating(IEnumerable<Review> reviews)
    {
        var list = reviews.ToList();
        return list.Count == 0 ? 0.0 : list.Average(r => r.Rating);
    }
}
