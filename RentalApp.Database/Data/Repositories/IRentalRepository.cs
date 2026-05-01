using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public interface IRentalRepository
{
    Task<List<Rental>> GetIncomingAsync(int ownerId);
    Task<List<Rental>> GetOutgoingAsync(int borrowerId);
    Task<Rental?> GetByIdAsync(int id);
    Task<Rental> CreateAsync(Rental rental);
    Task UpdateStatusAsync(int rentalId, RentalStatus status);
    Task<bool> HasOverlapAsync(int itemId, DateTime startDate, DateTime endDate, int? excludeRentalId = null);
    Task<List<Rental>> GetOverdueAsync();
}
