using System;

namespace SimRacingDashboard.Api.Models;

public class TrackModel
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Country { get; set; }
    public required string LayoutVersion { get; set; }
    public int? Turns { get; set; }
    public double? LengthKm { get; set; }
}