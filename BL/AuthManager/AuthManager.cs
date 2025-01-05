using BL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BL.AuthManager
{
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private readonly IDictionary<string, string> _users;
        private readonly SymmetricSecurityKey _securityKey;

        public AuthManager(IConfiguration configuration, ILogger<AuthManager> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new ArgumentException("JWT Key is not configured properly.");
            }

            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            _users = new Dictionary<string, string> // out to DB
           {
               { "testuser", "password" },
               { "testuser2", "password2345" }
           };
        }

        public string Authenticate(string username, string password)
        {
            if (!_users.TryGetValue(username, out var storedPassword) || storedPassword != password)
            {
                _logger.LogWarning("Authentication failed for user: {Username}", username);
                return null;
            }

            return GenerateJwtToken(username);
        }

        public string GenerateJwtToken(string username)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, username),
                   new Claim(ClaimTypes.Role, "User")
               }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Generated JWT token for user: {Username}", username);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = _securityKey
                }, out var validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Token validation failed: {Error}", ex.Message);
                return false;
            }
        }
    }
}

