using FootballMatchPrediction.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace FootballMatchPrediction.Data;

public class MatchPredictionDbContext : DbContext
{
    public DbSet<MatchPredictionResult> MatchPredictionResults { get; set; }

    public MatchPredictionDbContext(DbContextOptions<MatchPredictionDbContext> options) : base(options)
    {
    }
}