using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SimRacingDashboard.Api.Models;
using SimRacingDashboard.Api.Services;

namespace SimRacingDashboard.Api.Tests;

public class AuthHelper
{
    public static (string Key, string IV) GetEncryptionKeyAndIV()
    {
        // These are for development only
        return ("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=", "AAAAAAAAAAAAAAAAAAAAAA==");
    }

    public static EncryptionService GetEncryptionService()
    {
        (string key, string iv) = GetEncryptionKeyAndIV();
        return new EncryptionService(key, iv);
    }

    public static UserModel GetTestUser(string email = "test@test.com", string username = "Test User")
    {
        return new UserModel
        {
            Id = Guid.Parse("228764b1-cc23-4059-b2d8-f2d03d3c0ff9"), // Example fixed ID for testing
            Name = username.ToLowerInvariant(),
            DisplayName = username,
            Email = email.ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static EncryptedUserModel GetEncryptedTestUser(string email = "test@test.com", string username = "Test User", string password = "TestPassword123!")
    {
        (string key, string iv) = GetEncryptionKeyAndIV();
        var encryptionService = new EncryptionService(key, iv);
        var user = GetTestUser(email, username);
        var encryptedUser = user.Encrypt(encryptionService);
        encryptedUser.PasswordHash = new PasswordHasher<string>().HashPassword(user.Name, password);
        return encryptedUser;
    }

    public static string GetUserToken(UserModel user)
    {
        var key = "geheimer-schlüssel-1234567890-mit-mindestens-32-zeichen-dev"; // Use a secure key in production
        var issuer = "SimRacingDashboardDevApi";
        var audience = "dev";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("displayName", user.DisplayName),
        // Add other claims as needed
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
