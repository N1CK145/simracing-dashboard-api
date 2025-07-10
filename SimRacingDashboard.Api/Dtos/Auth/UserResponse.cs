using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimRacingDashboard.Api.Models;

namespace SimRacingDashboard.Api.Dtos.Auth;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }


    public UserResponse() { }

    public UserResponse(UserModel user)
    {
        Id = user.Id;
        Name = user.Name;
        DisplayName = user.DisplayName;
        Email = user.Email;
        CreatedAt = user.CreatedAt;
        LastLoginAt = user.LastLoginAt;
        IsActive = user.IsActive;
        ProfilePictureUrl = user.ProfilePictureUrl;
        Bio = user.Bio;
    }
}
