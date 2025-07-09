using SimRacingDashboard.Api;

namespace SimRacingDashboard.Api.Tests;

public class TrackTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TrackTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");
        response.EnsureSuccessStatusCode();
    }
}
