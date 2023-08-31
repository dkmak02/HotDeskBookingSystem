using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class DeleteLocation : Endpoint<DeleteLocationRequest>
{
    private readonly IMongo _mongo;
    
    public DeleteLocation(IMongo mongo)
    {
        _mongo = mongo;
    }
    public override void Configure()
    {
        Delete("locations/{locationName}");
        Roles("admin");
    }

    public override async Task HandleAsync(DeleteLocationRequest req, CancellationToken ct)
    {
        var location = await _mongo.Conn<LocationModel>("locations").Find(x => x.LocationName == req.locationName).FirstOrDefaultAsync();
        if (location == null)
        {
            ThrowError("Location not found", StatusCodes.Status404NotFound);
            return;
        }
        if(location.Desks.Count > 0)
        {
            ThrowError("Location still has desks", StatusCodes.Status400BadRequest);
            return;
        }
        await _mongo.Conn<LocationModel>("locations").DeleteOneAsync(x => x.LocationName == req.locationName);
        await SendAsync(new
        {
            message = "Location deleted"
        });
        
    }
}