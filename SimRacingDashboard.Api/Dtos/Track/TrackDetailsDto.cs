using SimRacingDashboard.Api.Models;

namespace SimRacingDashboard.Api.Dtos.Track;

public class TrackDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Location { get; set; }
    public string? Country { get; set; }
    public string? LayoutVersion { get; set; }
    public int? Turns { get; set; }
    public double? LengthKm { get; set; }

    public TrackDetailsDto() { }
    public TrackDetailsDto(TrackModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Location = model.Location;
        Country = model.Country;
        LayoutVersion = model.LayoutVersion;
        Turns = model.Turns;
        LengthKm = model.LengthKm;
    }
}
