using System;

namespace SimRacingDashboard.Api.Models;

public class DriverModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserModel? User { get; set; }
    public RaceDriverType DriverType { get; set; }
}
