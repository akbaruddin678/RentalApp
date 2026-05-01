using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Models;
namespace RentalApp.Database.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Category> Categories => Set<Category>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(e => { e.HasIndex(u => u.Email).IsUnique(); });
        modelBuilder.Entity<Item>(e => {
            e.HasOne(i => i.Owner).WithMany().HasForeignKey(i => i.OwnerId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });
        modelBuilder.Entity<Rental>(e => {
            e.HasOne(r => r.Item).WithMany(i => i.Rentals).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Borrower).WithMany().HasForeignKey(r => r.BorrowerId).OnDelete(DeleteBehavior.Restrict);
            e.Property(r => r.Status).HasConversion<string>();
        });
        modelBuilder.Entity<Review>(e => {
            e.HasOne(r => r.Item).WithMany(i => i.Reviews).HasForeignKey(r => r.ItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Reviewer).WithMany().HasForeignKey(r => r.ReviewerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Rental).WithOne(rental => rental.Review).HasForeignKey<Review>(r => r.RentalId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Category>().HasData(
            new Category { Id=1, Name="Tools", Description="Power and hand tools" },
            new Category { Id=2, Name="Camping Gear", Description="Outdoor and camping equipment" },
            new Category { Id=3, Name="Board Games", Description="Board games and puzzles" },
            new Category { Id=4, Name="Sports", Description="Sports and fitness equipment" },
            new Category { Id=5, Name="Electronics", Description="Electronic devices and gadgets" },
            new Category { Id=6, Name="Garden", Description="Gardening tools and equipment" });
    }
}
