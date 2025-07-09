using Microsoft.EntityFrameworkCore;
using SimRacingDashboard.Api.Models;

namespace SimRacingDashboard.Api.Data;

public class SimRacingDbContext : DbContext
{
    public SimRacingDbContext(DbContextOptions<SimRacingDbContext> options) : base(options) { }

    public DbSet<TrackModel> Tracks => Set<TrackModel>();
}
