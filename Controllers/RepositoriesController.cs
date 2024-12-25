using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitRepositoriesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        public RepositoriesController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [Authorize]
        [HttpGet("SearchRepositories")]
        public async Task<IActionResult> SearchRepositories(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("The 'query' parameter is required.");
            }

            try
            {
                var result = await _gitHubService.SearchRepositoriesAsync(query, page, perPage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
