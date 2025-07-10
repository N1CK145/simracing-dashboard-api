using System;
using SimRacingDashboard.Api.Models;

namespace SimRacingDashboard.Api.Dtos.Track;

public class CreateTrackRequest
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Country { get; set; }
    public string LayoutVersion { get; set; } = "GP";
    public int? Turns { get; set; }
    public double? LengthKm { get; set; }

    public TrackModel ToModel()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = Name,
            Location = Location,
            Country = Country,
            LayoutVersion = LayoutVersion,
            Turns = Turns,
            LengthKm = LengthKm
        };
    }
}
