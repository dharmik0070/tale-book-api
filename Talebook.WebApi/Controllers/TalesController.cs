using Microsoft.AspNetCore.Mvc;
using Talebook.Application.Interfaces;

namespace Talebook.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TalesController : ControllerBase
    {
        private readonly INewsService _newsService;

        public TalesController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopTales([FromQuery] int count = 20)
        {
            var tales = await _newsService.GetTopTalesAsync(count);
            return Ok(tales);
        }
    }
}
