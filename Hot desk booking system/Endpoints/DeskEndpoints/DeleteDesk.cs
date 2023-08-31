using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class DeleteDesk : Endpoint<DeleteDeskRequest>
{
    private readonly IMongo _mongo;
    private readonly ICheckBooked _checkBooked;
    
    public DeleteDesk(IMongo mongo, ICheckBooked checkBooked)
    {
        _mongo = mongo;
        _checkBooked = checkBooked;
    }
    public override void Configure()
    {
        Delete("desks/{locationName}/{deskName}");
        Roles("admin");
    }
    public override async Task HandleAsync(DeleteDeskRequest request, CancellationToken cancellationToken)
    {
        var desk = await _mongo.Conn<DeskModel>("desks")
            .Find(x => x.DeskName == request.deskName && x.LocationName == request.locationName)
            .FirstOrDefaultAsync();
        var location = await _mongo.Conn<LocationModel>("locations")
            .Find(x => x.LocationName == request.locationName)
            .FirstOrDefaultAsync();
        if (desk == null)
        {
            ThrowError("Desk not found in this location", StatusCodes.Status404NotFound);
            return;
        }
        if(location == null)
        {
            ThrowError("Location not found", StatusCodes.Status404NotFound);
            return;
        }
        if(_checkBooked.ActiveBooking(desk.Id).Result)
        {
            ThrowError("Desk is booked", StatusCodes.Status400BadRequest);
            return;
        }
        await _mongo.Conn<DeskModel>("desks").DeleteOneAsync(x => x.Id == desk.Id);
        location.Desks.Remove(desk.Id);
        var update = Builders<LocationModel>.Update.Set(l => l.Desks, location.Desks);
        await _mongo.Conn<LocationModel>("locations").UpdateOneAsync(l => l.Id == location.Id, update);
        await SendAsync(new
        {
            message = "Desk deleted"
        });

    }
    
         
}