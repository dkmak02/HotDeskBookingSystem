using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.UserEndpoints;

public class LogIn : Endpoint<LogInRequest>
{
    private readonly IAuthService _authService;
    private readonly IMongo _mongo;
    public LogIn(IAuthService authService, IMongo mongo)
    {
        _authService = authService;
        _mongo = mongo;
    }
    public override void Configure()
    {
        Post("users/login");
        AllowAnonymous();
    }
    public override async Task HandleAsync(LogInRequest req, CancellationToken ct)
    {
        if (!await _authService.CredentialsAreVaild(req.email, req.password))
        {
            ThrowError("Invalid credentials", StatusCodes.Status401Unauthorized);
        }
        var user = await _mongo.Conn<UserModel>("users").Find(x => x.Email == req.email).FirstOrDefaultAsync();
        var token = _authService.GenerateToken(user.Id, user.Role);
        HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");
        await SendAsync(new
        {
            token,
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            email = user.Email
        });
    }
}