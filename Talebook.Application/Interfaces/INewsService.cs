using Talebook.Application.DTOs;

namespace Talebook.Application.Interfaces
{
    public interface INewsService
    {
        Task<List<TaleDto>> GetTopTalesAsync(int count);
    }
}
