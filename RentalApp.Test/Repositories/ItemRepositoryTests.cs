using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Test.Fixtures;
using System.Threading.Tasks;
using Xunit;

namespace RentalApp.Test.Repositories;

public class ItemRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new ItemRepository(_fixture.Context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddItemToDatabase()
    {
        // Arrange
        var item = new Item { Title = "Test Drill", DailyRate = 10.00m, OwnerId = 1 };

        // Act
        var createdItem = await _repository.CreateAsync(item);

        // Assert
        Assert.NotEqual(0, createdItem.Id);
        Assert.Equal("Test Drill", createdItem.Title);
        
        var fetchedItem = await _repository.GetByIdAsync(createdItem.Id);
        Assert.NotNull(fetchedItem);
    }
}
