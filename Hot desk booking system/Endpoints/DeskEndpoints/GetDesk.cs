using FastEndpoints;
using Hot_desk_booking_system.Models;
using Hot_desk_booking_system.Models.ResponseModels;
using Hot_desk_booking_system.Services;
using Hot_desk_booking_system.Services.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Hot_desk_booking_system.Endpoints.DeskEndpoints;

public class DeskInfo
{
    public string Id { get; set; }
    public string DeskName { get; set; }
    public string LocationName { get; set; }
    public bool IsBooked { get; set; }
    
    public string? BookedBy { get; set; }
}
public class GetDesk : EndpointWithoutRequest
{
    private readonly IMongo _mongo;
    private readonly ICheckBooked _checkBooked;
    public GetDesk(IMongo mongo, ICheckBooked checkBooked)
    {
        _mongo = mongo;
        _checkBooked = checkBooked;
    }
    
    public override void Configure()
    {
        Get("desks");
    }
    
    public override async Task HandleAsync(CancellationToken ct)
    {
        
        var location = Query<string>("location", isRequired: false);
        var isBooked = Query<string>("isBooked", isRequired: false);
        var filter = Builders<DeskModel>.Filter.Empty;
        if (location != null)
        {
            filter = filter & Builders<DeskModel>.Filter.Eq(x => x.LocationName, location);
        }
        var desks = await _mongo.Conn<DeskModel>("desks").Find(filter).ToListAsync();
        var list = new List<DeskInfo>();
        foreach (var desk in desks)
        {
            var booked = await _checkBooked.IsBooked(desk.Id, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
                var deskinfo = new DeskInfo
                {
                    Id = desk.Id,
                    DeskName = desk.DeskName,
                    LocationName = desk.LocationName,
                    IsBooked = booked
                };
                if (HttpContext.User.IsInRole("admin") && booked)
                {
                    deskinfo.BookedBy = await _checkBooked.GetBookedBy(desk.Id, DateTime.UtcNow, DateTime.UtcNow);
                }
                list.Add(deskinfo);
                
        }
        
        if (isBooked != null)
        {
            list = list.Where(x => x.IsBooked == Convert.ToBoolean(isBooked)).ToList();
        }
        await SendAsync(list);   
        }
        
}