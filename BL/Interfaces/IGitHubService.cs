using Shared.Models;

namespace BL.Interfaces
{
    public interface IGitHubService
    {
        Task<GitHubSearchResult> SearchRepositoriesAsync(string query, int page = 1, int perPage = 15);
    }
}
