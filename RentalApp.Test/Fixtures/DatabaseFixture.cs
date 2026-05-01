using Microsoft.EntityFrameworkCore;
using RentalApp.Database.Data;
using RentalApp.Database.Models;
using System;

namespace RentalApp.Test.Fixtures;

public class DatabaseFixture : IDisposable
{
    public AppDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        Context = new AppDbContext(options);
        
        // Seed initial data
        Context.Users.Add(new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test@test.com", PasswordHash = "hash", PasswordSalt = "salt" });
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
