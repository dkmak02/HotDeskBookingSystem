using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;

namespace Hot_desk_booking_system.Services;

public class CheckBooked : ICheckBooked
{
    private readonly IMongo _mongo;
    public CheckBooked(IMongo mongo)
    {
        _mongo = mongo;
    }
    public async Task<bool>  IsBooked(string deskId, DateTime from, DateTime to)
    {
        var bookings = await _mongo.Conn<BookingModel>("bookings")
            .Find(x => x.DeskId == deskId && x.To > DateTime.UtcNow)
            .ToListAsync();
        var list = new List<bool>();
        foreach (var b in bookings)
        {
            if((b.From > from || b.To < from ) && (b.From > to || b.To < to)) {
                list.Add(true);
            }
        }

        if (list.All(x => x == false) && bookings.Count != 0)
        {
            return true;
        }
        return false;
    }

    public async Task<bool> ActiveBooking(string deskId)
    {
        var bookings = await _mongo.Conn<BookingModel>("bookings")
            .Find(x => x.DeskId == deskId && x.To < DateTime.UtcNow)
            .ToListAsync();
        if (bookings.Count != 0)
        {
            return true;
        }
        return false;
    }

    public Task<bool> IsMoreThan1Day(DateTime from)
    {
        if (from > DateTime.UtcNow && from - DateTime.UtcNow < TimeSpan.FromDays(1))
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public async Task<string> GetBookedBy(string deskId, DateTime from, DateTime to)
    {
        var bookings = await _mongo.Conn<BookingModel>("bookings")
            .Find(x => x.DeskId == deskId && x.To > DateTime.UtcNow)
            .ToListAsync();
        var booking = bookings.FirstOrDefault(x => x.From < from && x.To > to);
        var user = _mongo.Conn<UserModel>("users").Find(x => x.Id == booking.UserId).FirstOrDefaultAsync();
        if (user.Result != null)
        {
            return user.Result.FirstName + " " + user.Result.LastName;
        }
        return "null";
        
    }
}