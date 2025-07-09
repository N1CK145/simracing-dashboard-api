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

    [Fact]
    public async Task Put_UpdatesTrack_WhenValid()
    {
        // Create a track
        var createDto = new CreateTrackDto
        {
            Name = "Put Track",
            Location = "Put City",
            Country = "Putland"
        };
        var postResponse = await _client.PostAsJsonAsync("/api/track", createDto);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id = created!.Data!.Id;

        // Update the track
        var updateDto = new CreateTrackDto
        {
            Name = "Put Track Updated",
            Location = "Put City Updated",
            Country = "Putland Updated",
            LengthKm = 5.5,
            LayoutVersion = "v2",
            Turns = 12
        };
        var putResponse = await _client.PutAsJsonAsync($"/api/track/{id}", updateDto);
        putResponse.EnsureSuccessStatusCode();
        var apiResponse = await putResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Track updated successfully!", apiResponse.Message);
        Assert.Equal("Put Track Updated", apiResponse.Data!.Name);
    }

    [Fact]
    public async Task Put_ReturnsConflict_WhenDuplicate()
    {
        // Create two tracks
        var dto1 = new CreateTrackDto { Name = "PutDup1", Location = "Loc1", Country = "C1" };
        var dto2 = new CreateTrackDto { Name = "PutDup2", Location = "Loc2", Country = "C2" };
        var resp1 = await _client.PostAsJsonAsync("/api/track", dto1);
        var resp2 = await _client.PostAsJsonAsync("/api/track", dto2);
        var created2 = await resp2.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id2 = created2!.Data!.Id;

        // Try to update second track to have same name/location/country as first
        var updateDto = new CreateTrackDto { Name = "PutDup1", Location = "Loc1", Country = "C1" };
        var putResponse = await _client.PutAsJsonAsync($"/api/track/{id2}", updateDto);
        Assert.Equal(HttpStatusCode.Conflict, putResponse.StatusCode);
        var apiResponse = await putResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("already exists", apiResponse.Message);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenTrackDoesNotExist()
    {
        var updateDto = new CreateTrackDto { Name = "NoTrack", Location = "NoLoc", Country = "NoCountry" };
        var nonExistentId = Guid.NewGuid();
        var putResponse = await _client.PutAsJsonAsync($"/api/track/{nonExistentId}", updateDto);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenNullBody()
    {
        var nonExistentId = Guid.NewGuid();
        var putResponse = await _client.PutAsJsonAsync($"/api/track/{nonExistentId}", (CreateTrackDto?)null);
        Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
    }

    [Fact]
    public async Task Put_AllowsEmptyStrings_And_ZeroValues()
    {
        // Create a track
        var createDto = new CreateTrackDto { Name = "PutEmpty", Location = "PutLoc", Country = "PutCountry" };
        var postResponse = await _client.PostAsJsonAsync("/api/track", createDto);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id = created!.Data!.Id;

        // Update with empty/zero values
        var updateDto = new CreateTrackDto
        {
            Name = string.Empty,
            Location = string.Empty,
            Country = string.Empty,
            LengthKm = 0,
            LayoutVersion = string.Empty,
            Turns = 0
        };
        var putResponse = await _client.PutAsJsonAsync($"/api/track/{id}", updateDto);
        putResponse.EnsureSuccessStatusCode();
        var apiResponse = await putResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal(string.Empty, apiResponse.Data!.Name);
        Assert.Equal(0, apiResponse.Data!.LengthKm);
    }

    [Fact]
    public async Task Patch_UpdatesTrack_WhenValid()
    {
        // Create a track
        var createDto = new CreateTrackDto { Name = "Patch Track", Location = "Patch City", Country = "Patchland" };
        var postResponse = await _client.PostAsJsonAsync("/api/track", createDto);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id = created!.Data!.Id;

        // Patch the track
        var patchDto = new PatchTrackDto { Name = "Patch Track Updated", Turns = 15 };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/track/{id}", patchDto);
        patchResponse.EnsureSuccessStatusCode();
        var apiResponse = await patchResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Track updated successfully!", apiResponse.Message);
        Assert.Equal("Patch Track Updated", apiResponse.Data!.Name);
        Assert.Equal(15, apiResponse.Data!.Turns);
    }

    [Fact]
    public async Task Patch_ReturnsConflict_WhenDuplicate()
    {
        // Create two tracks
        var dto1 = new CreateTrackDto { Name = "PatchDup1", Location = "Loc1", Country = "C1" };
        var dto2 = new CreateTrackDto { Name = "PatchDup2", Location = "Loc2", Country = "C2" };
        var resp1 = await _client.PostAsJsonAsync("/api/track", dto1);
        var resp2 = await _client.PostAsJsonAsync("/api/track", dto2);
        var created2 = await resp2.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id2 = created2!.Data!.Id;

        // Try to patch second track to have same name/location/country as first
        var patchDto = new PatchTrackDto { Name = "PatchDup1", Location = "Loc1", Country = "C1" };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/track/{id2}", patchDto);
        Assert.Equal(HttpStatusCode.Conflict, patchResponse.StatusCode);
        var apiResponse = await patchResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("already exists", apiResponse.Message);
    }

    [Fact]
    public async Task Patch_ReturnsNotFound_WhenTrackDoesNotExist()
    {
        var patchDto = new PatchTrackDto { Name = "NoTrack" };
        var nonExistentId = Guid.NewGuid();
        var patchResponse = await _client.PatchAsJsonAsync($"/api/track/{nonExistentId}", patchDto);
        Assert.Equal(HttpStatusCode.NotFound, patchResponse.StatusCode);
    }

    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenNullBody()
    {
        var nonExistentId = Guid.NewGuid();
        var patchResponse = await _client.PatchAsJsonAsync($"/api/track/{nonExistentId}", (PatchTrackDto?)null);
        Assert.Equal(HttpStatusCode.BadRequest, patchResponse.StatusCode);
    }

    [Fact]
    public async Task Patch_AllowsEmptyStrings_And_ZeroValues()
    {
        // Create a track
        var createDto = new CreateTrackDto { Name = "PatchEmpty", Location = "PatchLoc", Country = "PatchCountry" };
        var postResponse = await _client.PostAsJsonAsync("/api/track", createDto);
        postResponse.EnsureSuccessStatusCode();
        var created = await postResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        var id = created!.Data!.Id;

        // Patch with empty/zero values
        var patchDto = new PatchTrackDto
        {
            Turns = 5,
            LengthKm = 6
        };
        var patchResponse = await _client.PatchAsJsonAsync($"/api/track/{id}", patchDto);
        patchResponse.EnsureSuccessStatusCode();

        var apiResponse = await patchResponse.Content.ReadFromJsonAsync<ApiResponse<TrackDetailsDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal(createDto.Name, apiResponse.Data!.Name);
        Assert.Equal(5, apiResponse.Data!.Turns);
        Assert.Equal(6, apiResponse.Data!.LengthKm);
    }
}
