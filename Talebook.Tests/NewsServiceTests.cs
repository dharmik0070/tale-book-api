using FluentAssertions;
using HackerNews.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

public class NewsServiceTests
{
    private readonly IMemoryCache _memoryCache;

    public NewsServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [Fact]
    public async Task GetTopNewsAsync_ReturnsListOfNews()
    {
        // Arrange
        var storyIds = new List<int> { 1, 2 };
        var response1 = new { title = "Title 1", url = "http://url1.com" };
        var response2 = new { title = "Title 2", url = "http://url2.com" };

        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = JsonContent.Create(storyIds) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = JsonContent.Create(response1) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = JsonContent.Create(response2) });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/")
        };

        var service = new NewsService(httpClient, _memoryCache, Mock.Of<ILogger<NewsService>>());

        // Act
        var result = await service.GetTopNewsAsync(2);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(r => r.Title == "Title 1");
        result.Should().ContainSingle(r => r.Title == "Title 2");
    }

    [Fact]
    public async Task GetTopNewsAsync_SkipsInvalidStories()
    {
        var storyIds = new List<int> { 1 };
        var response1 = new { title = "", url = "" }; // invalid story

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = JsonContent.Create(storyIds) })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = JsonContent.Create(response1) });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/")
        };

        var service = new NewsService(httpClient, _memoryCache, Mock.Of<ILogger<NewsService>>());

        var result = await service.GetTopNewsAsync(1);

        result.Should().BeEmpty();
    }

}
