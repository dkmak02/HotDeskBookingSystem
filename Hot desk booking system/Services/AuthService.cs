using FastEndpoints.Security;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Services;

public class AuthService : IAuthService
{
    private readonly IMongo _mongo;
    private readonly IConfiguration _configuration;
    public AuthService(IMongo mongo , IConfiguration configuration) {
        _mongo = mongo;
        _configuration = configuration;
    }
    public async Task<bool> CredentialsAreVaild(string username, string password)
    {
        var con = _mongo.Conn<UserModel>("users");
        var user = (await con.FindAsync(x => x.Email == username)).FirstOrDefault();
        if (user == null)
        {
            return false;
        };
        if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
        {
            return false;
        }
        return true;
    }

    public string GenerateToken(string id, string role)
    {
        try
        {
            var to = JWTBearer.CreateToken(
                signingKey: _configuration.GetSection("JWTSecet").Value,
                expireAt: DateTime.Now.AddHours(1),
                privileges: u =>
                {
                    u.Claims.Add(new("id", id));
                    u.Roles.Add(role);
                });
            return to;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


    }
}