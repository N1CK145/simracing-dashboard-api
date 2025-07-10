using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimRacingDashboard.Api.Data;
using SimRacingDashboard.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SimRacingDashboard.Api.Services;

public class AuthService
{
    private readonly SimRacingDbContext context;
    private readonly EncryptionService _encryptionService;
    private readonly PasswordHasher<string> _hasher;
    private readonly IConfiguration _config;

    public AuthService(SimRacingDbContext _context, EncryptionService encryptionService, IConfiguration config)
    {
        context = _context;
        _encryptionService = encryptionService;
        _hasher = new PasswordHasher<string>();
        _config = config;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var encryptedEmail = _encryptionService.Encrypt(email.ToLowerInvariant());
        var user = await context.Users.Where(user => user.EncryptedEmail == encryptedEmail).FirstOrDefaultAsync();

        if (user == null)
            return null;

        if (_hasher.VerifyHashedPassword(email.ToLowerInvariant(), user.PasswordHash, password) == PasswordVerificationResult.Failed)
            return null;

        var decryptedUser = user.Decrypt(_encryptionService);
        var token = GenerateJwt(decryptedUser);

        user.LastLoginAt = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<UserModel?> RegisterAsync(string email, string username, string password)
    {
        var encryptedEmail = _encryptionService.Encrypt(email.ToLowerInvariant());
        var existingUser = await context.Users.Where(user => user.EncryptedEmail == encryptedEmail).FirstOrDefaultAsync();

        if (existingUser != null)
            return null;

        var newUser = new UserModel
        {
            Id = Guid.NewGuid(),
            Name = username.ToLowerInvariant(),
            DisplayName = username,
            Email = email.ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = null,
            IsActive = true,
            ProfilePictureUrl = null,
            Bio = null
        };

        var encryptedUser = newUser.Encrypt(_encryptionService);
        encryptedUser.PasswordHash = _hasher.HashPassword(email.ToLowerInvariant(), password);

        await context.Users.AddAsync(encryptedUser);
        await context.SaveChangesAsync();
        return newUser;
    }

    public async Task<UserModel?> GetCurrentUserAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
            return null;

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return null;

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await context.Users.FindAsync(userId);

        if (user == null)
            return null;

        return user.Decrypt(_encryptionService);
    }

    private string GenerateJwt(UserModel user)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var base64Key = Convert.ToBase64String(keyBytes);
        var key = new SymmetricSecurityKey(Convert.FromBase64String(base64Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("CreatedAt", user.CreatedAt.ToString("o")),
            new Claim("LastLoginAt", user.LastLoginAt?.ToString("o") ?? string.Empty),
            new Claim("ProfilePictureUrl", user.ProfilePictureUrl ?? string.Empty),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
