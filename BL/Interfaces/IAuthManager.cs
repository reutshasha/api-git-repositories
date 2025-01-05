namespace BL.Interfaces
{
    public interface IAuthManager
    {
        string Authenticate(string username, string password);
        string GenerateJwtToken(string username);
        Task<bool> ValidateToken(string token);
    }
}
