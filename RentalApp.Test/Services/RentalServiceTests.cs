using RentalApp.Database.Data.Repositories;
using RentalApp.Database.Models;
using RentalApp.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RentalApp.Test.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _mockRepo;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _mockRepo = new Mock<IRentalRepository>();
        _service = new RentalService(_mockRepo.Object);
    }

    [Theory]
    [InlineData(RentalStatus.Requested, RentalStatus.Approved, true)]
    [InlineData(RentalStatus.Requested, RentalStatus.Completed, false)]
    [InlineData(RentalStatus.OutForRent, RentalStatus.Returned, true)]
    public void CanTransitionTo_ShouldValidateCorrectly(RentalStatus current, RentalStatus next, bool expected)
    {
        // Act
        var result = _service.CanTransitionTo(current, next);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateTotalPrice_ShouldMultiplyRateByDays()
    {
        // Arrange
        var rate = 15.00m;
        var start = DateTime.Today;
        var end = DateTime.Today.AddDays(3);

        // Act
        var total = _service.CalculateTotalPrice(rate, start, end);

        // Assert
        Assert.Equal(45.00m, total);
    }
}
