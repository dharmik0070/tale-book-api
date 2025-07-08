using HackerNews.Application.DTOs;

namespace HackerNews.Application.Interfaces
{
    public interface INewsService
    {
        Task<List<NewsItemDto>> GetTopNewsAsync(int count);
    }
}
