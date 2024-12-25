using BL.DTOs;
using BL.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace GitRepositoriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login attempt with missing data.");
                return BadRequest("Invalid username or password.");
            }

            var token = _authManager.Authenticate(loginDto.Username, loginDto.Password);

            if (token == null)
            {
                _logger.LogWarning("Unauthorized login attempt for username: {Username}", loginDto.Username);
                return Unauthorized("Invalid username or password.");
            }

            _logger.LogInformation("User {Username} logged in successfully.", loginDto.Username);
            return Ok(new { Token = token });
        }
    }

}


