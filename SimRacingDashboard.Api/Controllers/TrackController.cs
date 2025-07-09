using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimRacingDashboard.Api.Dtos;
using SimRacingDashboard.Api.Data;
using SimRacingDashboard.Api.Dtos.Track;

namespace SimRacingDashboard.Api.Controllers
{
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
            var tracksDtos = tracks.Select(t => new TrackDto(t));
            return Ok(ApiResponse<IEnumerable<TrackDto>>.Ok(tracksDtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var track = await db.Tracks.FindAsync(id);

            if (track == null)
                return NotFound();

            return Ok(ApiResponse<TrackDetailsDto>.Ok(new TrackDetailsDto(track)));
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateTrackDto newTrack)
        {
            var trackExists = await db.Tracks.AnyAsync(existing => existing.Name == newTrack.Name && existing.Location == newTrack.Location && existing.Country == newTrack.Country);

            if (trackExists)
                return Conflict(ApiResponse<TrackDetailsDto>.Fail("Track already exists!"));

            var entity = await db.Tracks.AddAsync(newTrack.ToModel());
            await db.SaveChangesAsync();

            var dto = new TrackDetailsDto(entity.Entity);

            return CreatedAtAction(
                nameof(Get),
                new { id = dto.Id },
                ApiResponse<TrackDetailsDto>.Ok(dto, "Track created successfully!")
            );
        }
    }
}
