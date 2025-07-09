using System.Net;
using System.Net.Http.Json;
using SimRacingDashboard.Api.Dtos;
using SimRacingDashboard.Api.Dtos.Track;

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

    [Fact]
    public async Task Get_ReturnsListOfTracks()
    {
        // GIVEN the API is running
        // WHEN a GET request is made to /api/track
        var response = await _client.GetAsync("/api/track");

        // THEN it should return a list of all tracks as TrackDto objects, wrapped in an ApiResponse
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<TrackDto>>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
    }

    [Fact]
    public async Task Get_ById_ReturnsTrackDetails_WhenTrackExists()
    {
        // GIVEN a track exists (create one first)
        var createDto = new CreateTrackDto
        {
            Name = "Test Track",
            Location = "Test City",
            Country = "Testland"
        };
        var postResponse = await _client.PostAsJsonAsync("/api/track", createDto);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id = created!.Data!.Id;

        // WHEN a GET request is made to /api/track/{id}
        var response = await _client.GetAsync($"/api/track/{id}");

        // THEN it should return the details of the track
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Test Track", apiResponse.Data!.Name);
    }

    [Fact]
    public async Task Get_ById_ReturnsNotFound_WhenTrackDoesNotExist()
    {
        // GIVEN the API is running and a track with a specific ID does not exist
        var nonExistentId = Guid.NewGuid();

        // WHEN a GET request is made to /api/track/{id}
        var response = await _client.GetAsync($"/api/track/{nonExistentId}");

        // THEN it should return a 404 Not Found response
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesTrack_WhenUnique()
    {
        // GIVEN the API is running and a new, unique track is submitted
        var createDto = new CreateTrackDto
        {
            Name = "Unique Track",
            Location = "Unique City",
            Country = "Uniqueland"
        };

        // WHEN a POST request is made to /api/track
        var response = await _client.PostAsJsonAsync("/api/track", createDto);

        // THEN it should create the track and return 201 Created
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Track created successfully!", apiResponse.Message);
        Assert.Equal("Unique Track", apiResponse.Data!.Name);
    }

    [Fact]
    public async Task Post_ReturnsConflict_WhenDuplicate()
    {
        // GIVEN a track already exists
        var createDto = new CreateTrackDto
        {
            Name = "Duplicate Track",
            Location = "Duplicate City",
            Country = "Duplicateland"
        };
        await _client.PostAsJsonAsync("/api/track", createDto);

        // WHEN a POST request is made with the same data
        var response = await _client.PostAsJsonAsync("/api/track", createDto);

        // THEN it should return 409 Conflict
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal("Track already exists!", apiResponse.Message);
    }
}
