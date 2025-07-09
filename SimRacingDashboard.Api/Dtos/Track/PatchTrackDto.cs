using System;

namespace SimRacingDashboard.Api.Dtos.Track;

public class PatchTrackDto
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Country { get; set; }
    public string? LayoutVersion { get; set; }
    public int? Turns { get; set; }
    public double? LengthKm { get; set; }
}
