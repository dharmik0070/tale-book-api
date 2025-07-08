using Microsoft.AspNetCore.Mvc;
using Moq;
using HackerNews.Application.DTOs;
using HackerNews.Application.Interfaces;
using HackerNews.WebApi.Controllers;
using Microsoft.Extensions.Logging;

public class NewsControllerTests
{
    [Fact]
    public async Task GetTopNews_WithCustomCount_ReturnsCorrectNumber()
    {
        // Arrange
        var mockService = new Mock<INewsService>();
        mockService.Setup(s => s.GetTopNewsAsync(2))
                   .ReturnsAsync(new List<NewsItemDto>
                   {
                   new NewsItemDto { Title = "News 1", Url = "http://1.com" },
                   new NewsItemDto { Title = "News 2", Url = "http://2.com" }
                   });

        var controller = new NewsController(mockService.Object, Mock.Of<ILogger<NewsController>>());

        // Act
        var result = await controller.GetTopNews(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var news = Assert.IsAssignableFrom<IEnumerable<NewsItemDto>>(okResult.Value);
        Assert.Equal(2, news.Count());
    }


    [Fact]
    public async Task GetTopNews_InvalidCount_ReturnsBadRequest()
    {
        // Arrange
        var mockService = new Mock<INewsService>();
        var controller = new NewsController(mockService.Object, Mock.Of<ILogger<NewsController>>());

        // Act
        var result = await controller.GetTopNews(0);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Count must be between 1 and 200.", badRequest.Value);
    }

    [Fact]
    public async Task GetTopNews_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        var mockService = new Mock<INewsService>();
        mockService.Setup(s => s.GetTopNewsAsync(It.IsAny<int>()))
                   .ThrowsAsync(new Exception("API failed"));

        var controller = new NewsController(mockService.Object, Mock.Of<ILogger<NewsController>>());

        var result = await controller.GetTopNews(5);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, errorResult.StatusCode);
        Assert.Equal("An unexpected error occurred.", errorResult.Value);
    }

    [Fact]
    public async Task GetTopNews_WhenHttpRequestFails_Returns502()
    {
        var mockService = new Mock<INewsService>();
        mockService.Setup(s => s.GetTopNewsAsync(It.IsAny<int>()))
                   .ThrowsAsync(new HttpRequestException("Network down"));

        var controller = new NewsController(mockService.Object, Mock.Of<ILogger<NewsController>>());

        var result = await controller.GetTopNews(10);

        var objResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(502, objResult.StatusCode);
        Assert.Equal("Failed to fetch data from external API.", objResult.Value);
    }

}