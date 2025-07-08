using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using Talebook.Application.DTOs;
using Talebook.Application.Interfaces;

namespace Talebook.Infrastructure.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private const string TopStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json";
        private const string StoryDetailUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";

        public NewsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<TaleDto>> GetTopTalesAsync(int count)
        {
            const string cacheKey = "TopTales";
            List<TaleDto> allTales;

            if (!_cache.TryGetValue(cacheKey, out allTales))
            {
                var ids = await _httpClient.GetFromJsonAsync<List<int>>(TopStoriesUrl);
                allTales = new List<TaleDto>();

                if (ids != null && ids.Count > 0)
                {
                    foreach (var id in ids.Take(200)) // Always fetch max once
                    {
                        var url = string.Format(StoryDetailUrl, id);
                        var story = await _httpClient.GetFromJsonAsync<HackerNewsStory>(url);

                        if (!string.IsNullOrEmpty(story?.Title) && !string.IsNullOrEmpty(story?.Url))
                        {
                            allTales.Add(new TaleDto { Title = story.Title, Url = story.Url });
                        }
                    }

                    // Cache full list
                    _cache.Set(cacheKey, allTales, TimeSpan.FromMinutes(10));
                }
            }

            // Return the requested count
            return allTales.Take(count).ToList();
        }


        private class HackerNewsStory
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}
