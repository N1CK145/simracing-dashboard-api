using System;

namespace SimRacingDashboard.Api.Models;

public class RaceIncidentModel
{
    public Guid Id { get; set; }
    public string Time { get; set; }
    public int Lap { get; set; }
    public DriverModel Driver { get; set; }
    public string Team { get; set; }
    public string Incident { get; set; }
    public string Penalty { get; set; }
}
