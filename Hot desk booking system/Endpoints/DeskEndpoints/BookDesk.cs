using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.RequestModels;
using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class BookDesk : Endpoint<BookDeskRequest>
{
    private readonly IMongo _mongo;
    private readonly ICheckBooked _checkBooked;
    
    public BookDesk(IMongo mongo, ICheckBooked checkBooked)
    {
        _mongo = mongo;
        _checkBooked = checkBooked;
    }
    public override void Configure()
    {
        Post("desks/{locationName}/{deskName}");
    }

    public override async Task HandleAsync(BookDeskRequest req, CancellationToken ct)
    {
        var desk = await _mongo.Conn<DeskModel>("desks")
            .Find(x => x.DeskName == req.deskName && x.LocationName == req.locationName)
            .FirstOrDefaultAsync();
        if(desk == null) {
            ThrowError("Desk not found in this location", StatusCodes.Status404NotFound);
            return;
        }
        if (req.days > 7)
        {
            ThrowError("You can only book a desk for 7 days", StatusCodes.Status400BadRequest);
            return;
        }

        var startDate = DateTime.Parse(req.from);
        var endDate = startDate.AddDays(req.days);
        if (_checkBooked.IsBooked(desk.Id, startDate, endDate).Result)
        {
            ThrowError("Desk is already booked", StatusCodes.Status400BadRequest);
            return;
        }
        var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        
        var booking = new BookingModel
        {
            DeskId = desk.Id,
            From = startDate,
            To = endDate,
            UserId = id
        };
        await _mongo.Conn<BookingModel>("bookings").InsertOneAsync(booking);
        await SendAsync(new
        {
            message = "Desk booked"
        });
        
    }
}