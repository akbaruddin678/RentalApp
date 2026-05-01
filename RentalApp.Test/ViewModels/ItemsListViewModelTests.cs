using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.ViewModels;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RentalApp.Test.ViewModels;

public class ItemsListViewModelTests
{
    [Fact]
    public async Task LoadItemsCommand_ShouldPopulateItemsList()
    {
        // Arrange
        var mockRepo = new Mock<IItemRepository>();
        mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Item> { new Item { Title = "Item 1" }, new Item { Title = "Item 2" } });
                
        var viewModel = new ItemsListViewModel(mockRepo.Object);

        // Act
        await viewModel.LoadItemsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, viewModel.Items.Count);
        Assert.False(viewModel.IsBusy);
        Assert.False(viewModel.HasError);
    }
}
