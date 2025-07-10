using SimRacingDashboard.Api.Models;

namespace SimRacingDashboard.Api.Dtos.Track;

public class TrackResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Country { get; set; }
    public string LayoutVersion { get; set; }

    public TrackResponse() { }
    public TrackResponse(TrackModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Location = model.Location;
        Country = model.Country;
        LayoutVersion = model.LayoutVersion;
    }
}
