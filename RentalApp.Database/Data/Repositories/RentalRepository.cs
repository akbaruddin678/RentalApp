using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public class RentalRepository : IRentalRepository
{
    private readonly AppDbContext _context;
    public RentalRepository(AppDbContext context) { _context = context; }
    public async Task<List<Rental>> GetIncomingAsync(int ownerId) =>
        await _context.Rentals.Include(r=>r.Item).Include(r=>r.Borrower)
            .Where(r=>r.Item.OwnerId==ownerId).OrderByDescending(r=>r.CreatedAt).ToListAsync();
    public async Task<List<Rental>> GetOutgoingAsync(int borrowerId) =>
        await _context.Rentals.Include(r=>r.Item).ThenInclude(i=>i.Owner)
            .Include(r=>r.Review).Where(r=>r.BorrowerId==borrowerId)
            .OrderByDescending(r=>r.CreatedAt).ToListAsync();
    public async Task<Rental?> GetByIdAsync(int id) =>
        await _context.Rentals.Include(r=>r.Item).ThenInclude(i=>i.Owner)
            .Include(r=>r.Borrower).Include(r=>r.Review)
            .FirstOrDefaultAsync(r=>r.Id==id);
    public async Task<Rental> CreateAsync(Rental rental)
    {
        rental.CreatedAt = rental.UpdatedAt = DateTime.UtcNow;
        _context.Rentals.Add(rental); await _context.SaveChangesAsync(); return rental;
    }
    public async Task UpdateStatusAsync(int rentalId, RentalStatus status)
    {
        var r = await _context.Rentals.FindAsync(rentalId);
        if (r is not null) { r.Status = status; r.UpdatedAt = DateTime.UtcNow; await _context.SaveChangesAsync(); }
    }
    public async Task<bool> HasOverlapAsync(int itemId, DateTime start, DateTime end, int? excludeId = null)
    {
        var q = _context.Rentals.Where(r=>r.ItemId==itemId &&
            r.Status!=RentalStatus.Rejected && r.Status!=RentalStatus.Completed &&
            r.StartDate<end && r.EndDate>start);
        if (excludeId.HasValue) q = q.Where(r=>r.Id!=excludeId.Value);
        return await q.AnyAsync();
    }
    public async Task<List<Rental>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Rentals.Include(r=>r.Item).Include(r=>r.Borrower)
            .Where(r=>r.Status==RentalStatus.OutForRent && r.EndDate<now).ToListAsync();
    }
}
