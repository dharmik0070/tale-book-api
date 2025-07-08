using Microsoft.AspNetCore.Mvc;
using Moq;
using Talebook.Application.DTOs;
using Talebook.Application.Interfaces;
using Talebook.WebApi.Controllers;

public class TalesControllerTests
{
    [Fact]
    public async Task GetTopTales_WithCustomCount_ReturnsCorrectNumber()
    {
        // Arrange
        var mockService = new Mock<INewsService>();
        mockService.Setup(s => s.GetTopTalesAsync(2))
                   .ReturnsAsync(new List<TaleDto>
                   {
                   new TaleDto { Title = "Tale 1", Url = "http://1.com" },
                   new TaleDto { Title = "Tale 2", Url = "http://2.com" }
                   });

        var controller = new TalesController(mockService.Object);

        // Act
        var result = await controller.GetTopTales(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tales = Assert.IsAssignableFrom<IEnumerable<TaleDto>>(okResult.Value);
        Assert.Equal(2, tales.Count());
    }
}