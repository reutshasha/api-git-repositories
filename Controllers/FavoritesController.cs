using BL.Interfaces;
using BL.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace GitRepositoriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _FavoriteService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavoriteController(IFavoriteService FavoriteService, IHttpContextAccessor httpContextAccessor)
        {
            _FavoriteService = FavoriteService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpPost("AddToBookmark")]
        public async Task<IActionResult> AddToBookmark([FromBody] GitHubRepository favorite)
        {
            if (favorite == null)
                return BadRequest("Invalid data for Bookmark");

            await _FavoriteService.AddBookmarkAsync(favorite);
            return Ok("Added to Bookmarks");
        }

        [Authorize]
        [HttpGet("GetBookmarkes")]

        public async Task<IActionResult> GetBookmarkes()
        {
            var Favorite = await _FavoriteService.GetBookmarkesAsync();
            return Ok(Favorite);
        }

        [Authorize]
        [HttpDelete("DeleteBookmark/{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            var favorite = await _FavoriteService.DeleteBookmarkAsync(id, _httpContextAccessor.HttpContext.Session);

            if (favorite == null)
                return BadRequest("Invalid data for Bookmark");
            {
                return Ok(new { message = "Bookmark removed successfully", favorite });
            }

        }
    }
}