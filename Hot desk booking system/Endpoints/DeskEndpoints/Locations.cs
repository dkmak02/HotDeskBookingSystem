using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class Locations : Endpoint<LocationsRequest>
{
    private readonly IMongo _mongo;

    public Locations(IMongo mongo)
    {
        _mongo = mongo;
    }
    public override void Configure()
    {
        Verbs(Http.POST, Http.GET);
        Routes("locations");
        Roles("admin", "user");
    }
    public override async Task HandleAsync(LocationsRequest request, CancellationToken cancellationToken)
    {
        if (HttpMethod == Http.GET)
        {
            var locations = await _mongo.Conn<LocationModel>("locations").Find(_ => true).ToListAsync();
            await SendAsync(locations);
        }
        else
        {
            var isAdmin = HttpContext.User.IsInRole("admin"); 

            if (!isAdmin)
            {
                ThrowError("You are not an admin", StatusCodes.Status403Forbidden);
                return;
            }
            var location = new LocationModel
            {
                LocationName = request.locationName
            };
            await _mongo.Conn<LocationModel>("locations").InsertOneAsync(location);
            await SendAsync(location);
        }
    }
}