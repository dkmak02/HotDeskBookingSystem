using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class ChangeDesk : Endpoint<ChangeDeskRequest>
{
    private readonly IMongo _mongo;
    private readonly ICheckBooked _checkBooked;
    
    public ChangeDesk(IMongo mongo, ICheckBooked checkBooked)
    {
        _checkBooked = checkBooked;
        _mongo = mongo;
    }
    public override void Configure()
    {
        Patch("desks/{locationName}/{deskName}/{bookingId}");
    }

    public override async Task HandleAsync(ChangeDeskRequest request, CancellationToken cancellationToken)
    {
        var desk = await _mongo.Conn<DeskModel>("desks")
            .Find(x => x.DeskName == request.deskName && x.LocationName == request.locationName)
            .FirstOrDefaultAsync();
        if(desk == null) {
            ThrowError("Desk not found in this location", StatusCodes.Status404NotFound);
            return;
        }
        var desk2 = await _mongo.Conn<DeskModel>("desks")
            .Find(x => x.DeskName == request.newDeskName && x.LocationName == request.locationName)
            .FirstOrDefaultAsync();
        if(desk2 == null) {
            ThrowError("New Desk not found in this location", StatusCodes.Status404NotFound);
            return;
        }
        var booking = await _mongo.Conn<BookingModel>("bookings")
            .Find(x => x.Id == request.bookingId)
            .FirstOrDefaultAsync();
        if(_checkBooked.IsBooked(desk2.Id, booking.From, booking.To).Result) {
            ThrowError(" New Desk is already booked", StatusCodes.Status400BadRequest);
            return;
        }
        if(_checkBooked.IsMoreThan1Day(booking.From).Result) {
            ThrowError("You can only change days more than 1 day before", StatusCodes.Status400BadRequest);
            return;
        }

        var update = Builders<BookingModel>.Update
            .Set(b => b.DeskId, desk2.Id);
        await _mongo.Conn<BookingModel>("bookings").UpdateOneAsync(d => d.Id == request.bookingId, update);
        await SendAsync(new
        { 
            message = "New Desk booked"
        });
        
    }
}