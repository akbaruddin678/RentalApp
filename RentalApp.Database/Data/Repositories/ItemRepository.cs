using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;
namespace RentalApp.Database.Data.Repositories;
public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;
    public ItemRepository(AppDbContext context) { _context = context; }
    public async Task<List<Item>> GetAllAsync() =>
        await _context.Items.Include(i=>i.Owner).Include(i=>i.Category)
            .Where(i=>i.IsAvailable).OrderByDescending(i=>i.CreatedAt).ToListAsync();
    public async Task<Item?> GetByIdAsync(int id) =>
        await _context.Items.Include(i=>i.Owner).Include(i=>i.Category)
            .Include(i=>i.Reviews).FirstOrDefaultAsync(i=>i.Id==id);
    public async Task<List<Item>> GetByOwnerAsync(int ownerId) =>
        await _context.Items.Include(i=>i.Category).Where(i=>i.OwnerId==ownerId)
            .OrderByDescending(i=>i.CreatedAt).ToListAsync();
    public async Task<List<Item>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
    {
        var radiusMeters = radiusKm * 1000;
        return await _context.Items.FromSqlRaw(@"
            SELECT * FROM items WHERE ""Latitude"" IS NOT NULL AND ""Longitude"" IS NOT NULL
            AND ""IsAvailable"" = true
            AND ST_DWithin(ST_MakePoint(""Longitude"",""Latitude"")::geography,
                ST_MakePoint({0},{1})::geography,{2})",
            longitude, latitude, radiusMeters)
            .Include(i=>i.Owner).Include(i=>i.Category).ToListAsync();
    }
    public async Task<List<Item>> SearchAsync(string? searchText, int? categoryId)
    {
        var q = _context.Items.Include(i=>i.Owner).Include(i=>i.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var t = searchText.ToLower();
            q = q.Where(i=>i.Title.ToLower().Contains(t)||i.Description.ToLower().Contains(t));
        }
        if (categoryId.HasValue) q = q.Where(i=>i.CategoryId==categoryId.Value);
        return await q.OrderByDescending(i=>i.CreatedAt).ToListAsync();
    }
    public async Task<Item> CreateAsync(Item item)
    {
        item.CreatedAt = item.UpdatedAt = DateTime.UtcNow;
        _context.Items.Add(item); await _context.SaveChangesAsync(); return item;
    }
    public async Task UpdateAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.Items.Update(item); await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item is not null) { _context.Items.Remove(item); await _context.SaveChangesAsync(); }
    }
}
