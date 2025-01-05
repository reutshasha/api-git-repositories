using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace GitRepositoriesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        private readonly IEmailSender _emailSender;

        public RepositoriesController(IGitHubService gitHubService, IEmailSender emailSender)
        {
            _gitHubService = gitHubService;
            _emailSender = emailSender;
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




        [HttpPost("sendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] Email email)
        {
            if (string.IsNullOrWhiteSpace(email.repositoryName))
                return BadRequest("Repository name cannot be empty.");
            if (string.IsNullOrWhiteSpace(email.recipientEmail))
                return BadRequest("Recipient email cannot be empty.");

            try
            {
                await _emailSender.SendEmail(email.recipientEmail, email.repositoryName);
                return Ok($"Email sent successfully to: {email.recipientEmail} for repository: {email.repositoryName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while sending the email: {ex.Message}");
            }
        }
    }
}
