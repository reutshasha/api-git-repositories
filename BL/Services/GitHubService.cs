using BL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Shared.Models;
using System.Text.Json;

namespace BL.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        public GitHubService(IHttpClientFactory httpClientFactory, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient("GitHubClient");
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<GitHubSearchResult> SearchRepositoriesAsync(string query, int page = 1, int perPage = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("cannot be null or empty.", nameof(query));

            string sessionKey = $"GitHubSearch-{query}-Page{page}-Size{perPage}";
            var cachedResult = _httpContextAccessor.HttpContext.Session.GetString(sessionKey);

            if (cachedResult != null)
            {
                return JsonSerializer.Deserialize<GitHubSearchResult>(cachedResult);
            }

            var url = $"search/repositories?q={query}&page={page}&per_page={perPage}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"GitHub API Error: {response.StatusCode}");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GitHubSearchResult>(content);

            _httpContextAccessor.HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(result));

            return result;
        }
    }
}
