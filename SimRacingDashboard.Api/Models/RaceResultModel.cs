using System;

namespace SimRacingDashboard.Api.Models;

public class RaceResultModel
{
    public Guid Id { get; set; }
    public int Position { get; set; }
    public string Driver { get; set; }
    public string Team { get; set; }
    public int Grid { get; set; }
    public int Stops { get; set; }
    public string BestLap { get; set; }      // e.g. "1:30,399"
    public string TotalTime { get; set; }    // e.g. "40:01,709" or "+0,387"
    public int Points { get; set; }
    public RaceDriverType DriverType { get; set; }   // e.g. "Spieler:in"
}
