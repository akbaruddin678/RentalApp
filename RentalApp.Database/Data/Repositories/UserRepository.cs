using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) { _context = context; }
    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role)
            .FirstOrDefaultAsync(u=>u.Id==id);
    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role)
            .FirstOrDefaultAsync(u=>u.Email==email);
    public async Task<List<User>> GetAllAsync() =>
        await _context.Users.Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role)
            .Where(u=>u.IsActive).ToListAsync();
    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Add(user); await _context.SaveChangesAsync(); return user;
    }
    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user); await _context.SaveChangesAsync();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var u = await _context.Users.FindAsync(id);
        if (u is null) return false;
        u.IsActive = false; u.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(); return true;
    }
}
