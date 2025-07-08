using HackerNews.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopNews([FromQuery] int count = 20)
        {
            if (count < 1 || count > 200)
                return BadRequest("Count must be between 1 and 200.");

            try
            {
                var news = await _newsService.GetTopNewsAsync(count);
                return Ok(news);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP error while fetching news.");
                return StatusCode(502, "Failed to fetch data from external API.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching top news.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
