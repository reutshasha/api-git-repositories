using BL.Interfaces;
using DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.Models;

namespace BL.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly FavoriteDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavoriteService(FavoriteDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> AddBookmarkAsync(GitHubRepository favorite)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session == null)
                {
                    return "Session is not available.";
                }

                var favoritesJson = session.GetString("Bookmarkes");
                List<GitHubRepository> favorites = string.IsNullOrEmpty(favoritesJson)
                    ? new List<GitHubRepository>()
                    : JsonConvert.DeserializeObject<List<GitHubRepository>>(favoritesJson) ?? new List<GitHubRepository>();

                if (favorites.Any(f => f.Id == favorite.Id))
                {
                    return "Repository already bookmarked.";
                }

                favorites.Add(favorite);
                session.SetString("Bookmarkes", JsonConvert.SerializeObject(favorites));

                if (!await _context.Favorite.AnyAsync(f => f.Id == favorite.Id))
                {
                    _context.favorite.Add(new Favorite
                    {
                        Id = favorite.Id,
                        Name = favorite.Name,
                        Description = favorite.Description,
                        Stargazers_count = favorite.Stargazers_count,
                        Avatar_url = favorite.Owner?.AvatarUrl ?? string.Empty, 

                    });
                    await _context.SaveChangesAsync();
                }

                return "Added to bookmarks.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<List<GitHubRepository>> GetBookmarkByIdAsync(string id)
        {
            return await _context.Favorite
                                 .ToListAsync();
        }
        public async Task<List<GitHubRepository>> GetBookmarkesAsync()
        {
            return await _context.Favorite
                                 .ToListAsync();
        }
        public async Task<GitHubRepository> DeleteBookmarkAsync(int id, ISession session)
        {
            var favorite = await _context.Favorite.FindAsync(id);

            if (favorite == null)
            {
                return null;
            }

            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();

            //var sessionBookmarkes = HttpContext.Session.Get<List<int>>("Bookmarkes") ?? new List<int>();

            //if (sessionBookmarkes.Contains(id))
            //{
            //    sessionBookmarkes.Remove(id);
            //    session.Set("Bookmarkes", sessionBookmarkes);
            //}

            return favorite;
        }
    }
}
