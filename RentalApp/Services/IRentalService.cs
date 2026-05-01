using RentalApp.Database.Models;
namespace RentalApp.Services;
public interface IRentalService
{
    Task<bool> CanRequestRentalAsync(int itemId, DateTime startDate, DateTime endDate, int? excludeRentalId = null);
    bool CanTransitionTo(RentalStatus currentStatus, RentalStatus newStatus);
    decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate);
    Task<int> DetectOverdueAsync();
    (bool IsValid, string Message) ValidateDates(DateTime startDate, DateTime endDate);
}
