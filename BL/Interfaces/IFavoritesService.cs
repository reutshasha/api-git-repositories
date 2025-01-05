using Microsoft.AspNetCore.Http;
using Shared.Models;

namespace BL.Interfaces
{
    public interface IFavoriteService
    {
        Task <string> AddBookmarkAsync(GitHubRepository favorite);
        Task<List<GitHubRepository>> GetBookmarkByIdAsync(string userId);
        Task<List<GitHubRepository>> GetBookmarkesAsync();
        Task<GitHubRepository> DeleteBookmarkAsync(int id, ISession session);
    }
}
