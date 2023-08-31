using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Models.ResponseModels;
using FastEndpoints;
namespace Hot_desk_booking_system.Mappers;

public class SignUpMapper : Mapper<SignUpRequest, SignUpResponse, UserModel>
{
    public override UserModel ToEntity(SignUpRequest req) => new UserModel
    {
        Email = req.email,
        Password = BCrypt.Net.BCrypt.EnhancedHashPassword(req.password),
        Role = "user",
        FirstName = req.firstName,
        LastName = req.lastName,
        IsActive = true
    };
}