using BL.Interfaces;
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

        public FavoriteController(IFavoriteService FavoriteService)
        {
            _FavoriteService = FavoriteService;
        }

        [Authorize]
        [HttpPost("AddToFavorite")]
        public async Task<IActionResult> AddToFavorite([FromBody] GitHubRepository favorite)
        {
            if (favorite == null)
                return BadRequest("Invalid data for favorite");

            try
            {
                await _FavoriteService.AddFavoriteAsync(favorite);
                return Ok("Added to Favorite");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpGet("GetFavorites")]

        public async Task<IActionResult> GetFavorites()
        {
            var Favorite = await _FavoriteService.GetFavoritesAsync();
            return Ok(Favorite);
        }

    }
}