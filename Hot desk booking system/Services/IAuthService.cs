namespace Hot_desk_booking_system.Services;

public interface IAuthService
{
    Task<bool> CredentialsAreVaild(string username, string password);
    string GenerateToken(string id, string role);
}