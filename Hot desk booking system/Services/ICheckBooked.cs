namespace Hot_desk_booking_system.Services;

public interface ICheckBooked
{ 
    Task<bool> IsBooked(string deskId, DateTime from, DateTime to);
    Task<bool> ActiveBooking(string deskId);
    
    Task<bool> IsMoreThan1Day(DateTime from);
    
    Task<string> GetBookedBy(string deskId, DateTime from, DateTime to);
}