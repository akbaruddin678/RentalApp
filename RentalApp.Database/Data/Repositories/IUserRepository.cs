using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}
