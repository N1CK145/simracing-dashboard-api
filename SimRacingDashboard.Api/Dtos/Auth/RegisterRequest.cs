using System;

namespace SimRacingDashboard.Api.Dtos.Auth;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
