using System;

namespace SimRacingDashboard.Api.Dtos.Auth;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
