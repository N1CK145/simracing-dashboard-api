using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimRacingDashboard.Api.Data;
using SimRacingDashboard.Api.Dtos;
using SimRacingDashboard.Api.Dtos.Auth;
using SimRacingDashboard.Api.Models;
using SimRacingDashboard.Api.Services;
using Xunit;

namespace SimRacingDashboard.Api.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly UserModel _testUser;
    private readonly string _testPassword = "TestPassword123!";
    private readonly SimRacingDbContext context;

    public AuthTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });
        _testUser = AuthHelper.GetTestUser();

        using var scope = factory.Services.CreateScope();
        ResetDatabase(scope);
        SeedTestUser(scope, _testUser, _testPassword);
    }

    private void ResetDatabase(IServiceScope scope)
    {
        var db = scope.ServiceProvider.GetRequiredService<SimRacingDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    private void SeedTestUser(IServiceScope scope, UserModel user, string password)
    {
        var db = scope.ServiceProvider.GetRequiredService<SimRacingDbContext>();

        var encryptedUser = user.Encrypt(AuthHelper.GetEncryptionService());
        encryptedUser.PasswordHash = new PasswordHasher<string>().HashPassword(user.Email, password);
        db.Users.Add(encryptedUser);
        db.SaveChanges();
    }

    private string SetLoginCookie(UserModel user)
    {
        string token = AuthHelper.GetUserToken(user);
        _client.DefaultRequestHeaders.Add("Cookie", $"jwt={token}");
        return token;
    }

    [Fact]
    public async Task Login_ReturnsBearerToken()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Email = _testUser.Email,
            Password = _testPassword
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values) ? string.Join(";", values) : null;
        Assert.Contains("jwt=ey", setCookie ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_ReturnsBearerTokenCookie()
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Email = _testUser.Email,
            Password = _testPassword
        };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data!.Token);
        Assert.NotEmpty(apiResponse.Data.Token);
        Assert.StartsWith("ey", apiResponse.Data.Token); // JWTs typically start with '
    }

    [Fact]
    public async Task Logout_ClearsCookie()
    {
        SetLoginCookie(_testUser);

        var response = await _client.PostAsync("/api/auth/logout", null);
        var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values) ? string.Join(";", values) : null;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("expires=", setCookie ?? "", StringComparison.OrdinalIgnoreCase);
        Assert.Contains("jwt=;", setCookie ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Logout_WithLoggedOutAccount_HandlesGracefuly()
    {
        var response = await _client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    }

    // [Fact]
    // public async Task Me_Endpoint_ReturnsUserInfo()
    // {
    //     var (token, _) = await LoginAsync();
    //     _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //     var response = await _client.GetAsync("/api/auth/me");
    //     response.EnsureSuccessStatusCode();
    //     var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
    //     Assert.NotNull(apiResponse);
    //     Assert.True(apiResponse.Success);
    //     Assert.Equal("test@user.com", apiResponse.Data!.Email);
    // }

    // [Fact]
    // public async Task Login_SetsCookie()
    // {
    //     var (_, response) = await LoginAsync();
    //     var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values) ? string.Join(";", values) : null;
    //     Assert.Contains(".AspNetCore.Cookies", setCookie ?? "");
    // }

    // [Fact]
    // public async Task Login_ReturnsBody()
    // {
    //     var (_, response) = await LoginAsync();
    //     var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
    //     Assert.NotNull(apiResponse);
    //     Assert.True(apiResponse.Success);
    //     Assert.NotNull(apiResponse.Data!.Token);
    // }

    // [Fact]
    // public async Task Me_WithCookie()
    // {
    //     await LoginAsync();
    //     var response = await _client.GetAsync("/api/auth/me");
    //     response.EnsureSuccessStatusCode();
    //     var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
    //     Assert.NotNull(apiResponse);
    //     Assert.True(apiResponse.Success);
    //     Assert.Equal("test@user.com", apiResponse.Data!.Email);
    // }

    // [Fact]
    // public async Task Me_WithHeader()
    // {
    //     var (token, _) = await LoginAsync();
    //     _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //     var response = await _client.GetAsync("/api/auth/me");
    //     response.EnsureSuccessStatusCode();
    //     var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
    //     Assert.NotNull(apiResponse);
    //     Assert.True(apiResponse.Success);
    //     Assert.Equal("test@user.com", apiResponse.Data!.Email);
    // }
}
