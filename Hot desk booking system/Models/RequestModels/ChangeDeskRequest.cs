namespace Hot_desk_booking_system.Models.RequestModels;

public class ChangeDeskRequest
{
    public string locationName { get; set; }
    public string deskName { get; set; }
    public string newDeskName { get; set; }
    public string bookingId { get; set; }
}