using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using HackerNews.Application.DTOs;
using HackerNews.Application.Interfaces;
using HackerNews.Infrastructure.Models;
using Microsoft.Extensions.Logging;

namespace HackerNews.Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private const string TopStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json";
        private const string StoryDetailUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

        private readonly ILogger<NewsService> _logger;

        public NewsService(HttpClient httpClient, IMemoryCache cache, ILogger<NewsService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<NewsItemDto>> GetTopNewsAsync(int count)
        {
            const string cacheKey = "TopNews";
            List<NewsItemDto> allNews;

            if (!_cache.TryGetValue(cacheKey, out allNews))
            {
                var ids = await _httpClient.GetFromJsonAsync<List<int>>(TopStoriesUrl);
                allNews = new List<NewsItemDto>();
                var resultLock = new object();

                if (ids != null && ids.Count > 0)
                {
                    var topIds = ids.Take(count);

                    await Parallel.ForEachAsync(topIds, async (id, token) =>
                    {
                        try
                        {
                            var url = string.Format(StoryDetailUrl, id);
                            var story = await _httpClient.GetFromJsonAsync<HackerNewsStory>(url, token);

                            if (!string.IsNullOrEmpty(story?.Title) && !string.IsNullOrEmpty(story?.Url))
                            {
                                var item = new NewsItemDto { Title = story.Title, Url = story.Url };
                                lock (resultLock)
                                {
                                    allNews.Add(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Failed to fetch story with ID {StoryId}", id);
                        }
                    });

                    _cache.Set(cacheKey, allNews, TimeSpan.FromMinutes(10));
                }
            }

            return allNews.Take(count).ToList();
        }

    }
}
