using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public interface IReviewRepository
{
    Task<List<Review>> GetByItemAsync(int itemId);
    Task<List<Review>> GetByUserAsync(int userId);
    Task<bool> ExistsForRentalAsync(int rentalId);
    Task<double> GetAverageRatingAsync(int itemId);
    Task<Review> CreateAsync(Review review);
}
