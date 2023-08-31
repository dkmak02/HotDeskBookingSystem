using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using FastEndpoints;
using Hot_desk_booking_system.Mappers;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Models.ResponseModels;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.UserEndpoints;

public class SignUp : Endpoint<SignUpRequest, SignUpResponse, SignUpMapper>
{
    private readonly IMongo _mongo;
    private readonly IAuthService _authService;
    public SignUp(IMongo mongo, IAuthService authService)
    {
        _mongo = mongo;
        _authService = authService;
    }
    public override void Configure()
    {
        Post("users/sign");
        AllowAnonymous();
    }
    public override async Task HandleAsync(SignUpRequest request, CancellationToken cancellationToken)
    {
        var user = await _mongo.Conn<UserModel>("users").Find(x => x.Email == request.email).FirstOrDefaultAsync();
        if (user != null)
        {
            ThrowError("User already exists", StatusCodes.Status409Conflict);
            return;
        }
        var newUser = Map.ToEntity(request);
        await _mongo.Conn<UserModel>("users").InsertOneAsync(newUser);
        var token =  _authService.GenerateToken(newUser.Id, newUser.Role);
        HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");
        await SendAsync(new()
        {
            token = token,
            id = newUser.Id,
            firstName = newUser.FirstName,
            lastName = newUser.LastName,
            email = newUser.Email
        });
    }
    
}