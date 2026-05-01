using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<Item?> GetByIdAsync(int id);
    Task<List<Item>> GetByOwnerAsync(int ownerId);
    Task<List<Item>> GetNearbyAsync(double latitude, double longitude, double radiusKm);
    Task<List<Item>> SearchAsync(string? searchText, int? categoryId);
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(int id);
}
