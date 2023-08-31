namespace Hot_desk_booking_system.Models.RequestModels;

public class SignUpRequest
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public string passwordConfirm { get; set; }
}