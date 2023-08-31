using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class Desk : Endpoint<DeskRequest>
{
    private readonly IMongo _mongo;

    public Desk(IMongo mongo)
    {
        _mongo = mongo;
    }

    public override void Configure()
    {
        Post("desks/{locationName}");
        Roles("admin");
    }

    public override async Task HandleAsync(DeskRequest request, CancellationToken cancellationToken)
    {
        var location = await _mongo.Conn<LocationModel>("locations").Find(x => x.LocationName == request.locationName)
            .FirstOrDefaultAsync();
        if (location == null)
        {
            ThrowError("Location not found", StatusCodes.Status404NotFound);
            return;
        }

        var desk = new DeskModel
        {
            DeskName = request.deskName,
            LocationName = request.locationName
        };
        try
        {
            await _mongo.Conn<DeskModel>("desks").InsertOneAsync(desk);
            location.Desks.Add(desk.Id);
            var update = Builders<LocationModel>.Update.Set(l => l.Desks, location.Desks);
            await _mongo.Conn<LocationModel>("locations").UpdateOneAsync(l => l.Id == location.Id, update);
            await SendAsync(desk);
        }
        catch (Exception e)
        {
            ThrowError(e.Message, StatusCodes.Status500InternalServerError);
            throw;
        }
    }
}