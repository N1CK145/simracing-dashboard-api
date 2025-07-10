using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimRacingDashboard.Api.Dtos;
using SimRacingDashboard.Api.Data;
using SimRacingDashboard.Api.Dtos.Track;
using Microsoft.AspNetCore.Authorization;

namespace SimRacingDashboard.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrackController : ControllerBase
    {
        private readonly SimRacingDbContext db;

        public TrackController(SimRacingDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tracks = await db.Tracks.ToListAsync();
            var tracksDtos = tracks.Select(t => new TrackResponse(t));
            return Ok(ApiResponse<IEnumerable<TrackResponse>>.Ok(tracksDtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var track = await db.Tracks.FindAsync(id);

            if (track == null)
                return NotFound();

            return Ok(ApiResponse<TrackDetailsResponse>.Ok(new TrackDetailsResponse(track)));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateTrackRequest newTrack)
        {
            var trackExists = await db.Tracks.AnyAsync(existing => existing.Name == newTrack.Name && existing.Location == newTrack.Location && existing.Country == newTrack.Country);

            if (trackExists)
                return Conflict(ApiResponse<TrackDetailsResponse>.Fail("Track already exists!"));

            var entity = await db.Tracks.AddAsync(newTrack.ToModel());
            await db.SaveChangesAsync();

            var dto = new TrackDetailsResponse(entity.Entity);

            return CreatedAtAction(
                nameof(Get),
                new { id = dto.Id },
                ApiResponse<TrackDetailsResponse>.Ok(dto, "Track created successfully!")
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CreateTrackRequest updatedTrack)
        {
            if (updatedTrack == null)
                return BadRequest(ApiResponse<TrackDetailsResponse>.Fail("Invalid track data!"));

            var track = await db.Tracks.FindAsync(id);

            if (track == null)
                return NotFound();

            // Check if the track already exists with the same name, location, and country
            var trackExists = await db.Tracks.AnyAsync(existing =>
                existing.Id != id &&
                existing.Name == updatedTrack.Name &&
                existing.Location == updatedTrack.Location &&
                existing.Country == updatedTrack.Country);

            if (trackExists)
                return Conflict(ApiResponse<TrackDetailsResponse>.Fail("Track with the same name, location, and country already exists!"));

            // Update the track properties
            track.LengthKm = updatedTrack.LengthKm;
            track.LayoutVersion = updatedTrack.LayoutVersion;
            track.Turns = updatedTrack.Turns;
            track.Name = updatedTrack.Name;
            track.Location = updatedTrack.Location;
            track.Country = updatedTrack.Country;

            db.Tracks.Update(track);
            await db.SaveChangesAsync();

            return Ok(ApiResponse<TrackDetailsResponse>.Ok(new TrackDetailsResponse(track), "Track updated successfully!"));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, PatchTrackRequest patch)
        {
            if (patch == null)
                return BadRequest(ApiResponse<TrackDetailsResponse>.Fail("Invalid track data!"));

            var track = await db.Tracks.FindAsync(id);

            if (track == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(patch.Name))
                track.Name = patch.Name;

            if (!string.IsNullOrWhiteSpace(patch.Location))
                track.Location = patch.Location;

            if (!string.IsNullOrWhiteSpace(patch.Country))
                track.Country = patch.Country;

            if (patch.LayoutVersion != null)
                track.LayoutVersion = patch.LayoutVersion;

            if (patch.Turns.HasValue)
                track.Turns = patch.Turns.Value;

            if (patch.LengthKm.HasValue)
                track.LengthKm = patch.LengthKm.Value;

            // Check if the track already exists with the same name, location, and country
            var trackExists = await db.Tracks.AnyAsync(existing =>
                existing.Id != id &&
                existing.Name == track.Name &&
                existing.Location == track.Location &&
                existing.Country == track.Country &&
                existing.LayoutVersion == track.LayoutVersion);

            if (trackExists)
                return Conflict(ApiResponse<TrackDetailsResponse>.Fail("Track with the same name, location, and country already exists!"));

            db.Tracks.Update(track);
            await db.SaveChangesAsync();

            return Ok(ApiResponse<TrackDetailsResponse>.Ok(new TrackDetailsResponse(track), "Track updated successfully!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var track = await db.Tracks.FindAsync(id);

            if (track == null)
                return NotFound();

            db.Tracks.Remove(track);
            await db.SaveChangesAsync();

            return Ok(ApiResponse<TrackDetailsResponse>.Ok(new TrackDetailsResponse(track), "Track deleted successfully!"));
        }
    }
}
