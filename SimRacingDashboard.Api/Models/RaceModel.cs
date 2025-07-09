using System;

namespace SimRacingDashboard.Api.Models;

public class RaceModel
{
    public Guid Id { get; set; }

    public TrackModel Track { get; set; }

    public DateTime Date { get; set; }

    public List<DriverModel> Driver { get; set; } = new();

    public string Country { get; set; }

    public IEnumerable<WeatherType> Weather { get; set; }

    public int Laps { get; set; }

    public TimeSpan Duration { get; set; }

    public List<RaceResultModel> Results { get; set; } = new();

    public List<RaceIncidentModel> Incidents { get; set; } = new();
}
