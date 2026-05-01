using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;
    public ReviewRepository(AppDbContext context) { _context = context; }
    public async Task<List<Review>> GetByItemAsync(int itemId) =>
        await _context.Reviews.Include(r=>r.Reviewer).Where(r=>r.ItemId==itemId)
            .OrderByDescending(r=>r.CreatedAt).ToListAsync();
    public async Task<List<Review>> GetByUserAsync(int userId) =>
        await _context.Reviews.Include(r=>r.Item).Where(r=>r.ReviewerId==userId)
            .OrderByDescending(r=>r.CreatedAt).ToListAsync();
    public async Task<bool> ExistsForRentalAsync(int rentalId) =>
        await _context.Reviews.AnyAsync(r=>r.RentalId==rentalId);
    public async Task<double> GetAverageRatingAsync(int itemId)
    {
        var reviews = await _context.Reviews.Where(r=>r.ItemId==itemId).ToListAsync();
        return reviews.Count==0 ? 0.0 : reviews.Average(r=>r.Rating);
    }
    public async Task<Review> CreateAsync(Review review)
    {
        review.CreatedAt = DateTime.UtcNow;
        _context.Reviews.Add(review); await _context.SaveChangesAsync(); return review;
    }
}
