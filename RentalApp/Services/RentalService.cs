using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
namespace RentalApp.Services;
public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    public RentalService(IRentalRepository rentalRepository) { _rentalRepository = rentalRepository; }
    public async Task<bool> CanRequestRentalAsync(int itemId, DateTime start, DateTime end, int? excludeId = null)
        => !await _rentalRepository.HasOverlapAsync(itemId, start, end, excludeId);
    public bool CanTransitionTo(RentalStatus current, RentalStatus next) => (current, next) switch
    {
        (RentalStatus.Requested, RentalStatus.Approved) => true,
        (RentalStatus.Requested, RentalStatus.Rejected) => true,
        (RentalStatus.Approved, RentalStatus.OutForRent) => true,
        (RentalStatus.OutForRent, RentalStatus.Overdue) => true,
        (RentalStatus.OutForRent, RentalStatus.Returned) => true,
        (RentalStatus.Overdue, RentalStatus.Returned) => true,
        (RentalStatus.Returned, RentalStatus.Completed) => true,
        _ => false
    };
    public decimal CalculateTotalPrice(decimal dailyRate, DateTime start, DateTime end)
        => dailyRate * Math.Max(1, (end-start).Days);
    public async Task<int> DetectOverdueAsync()
    {
        var overdue = await _rentalRepository.GetOverdueAsync();
        foreach (var r in overdue) await _rentalRepository.UpdateStatusAsync(r.Id, RentalStatus.Overdue);
        return overdue.Count;
    }
    public (bool IsValid, string Message) ValidateDates(DateTime start, DateTime end)
    {
        if (start.Date < DateTime.UtcNow.Date) return (false, "Start date cannot be in the past.");
        if (end <= start) return (false, "End date must be after start date.");
        if ((end-start).Days > 90) return (false, "Rental cannot exceed 90 days.");
        return (true, string.Empty);
    }
}
