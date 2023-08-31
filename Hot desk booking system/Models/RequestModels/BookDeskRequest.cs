namespace Hot_desk_booking_system.Models.RequestModels;

public class BookDeskRequest
{
    public string locationName { get; set; }
    public string deskName { get; set; }
    public int days { get; set; }
    public string from { get; set; }
}