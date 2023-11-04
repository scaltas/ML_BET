using FootballMatchPrediction.Data.Model;

namespace FootballMatchPrediction.Data.Repository;

public class MatchPredictionRepository : IMatchPredictionRepository
{
    private readonly MatchPredictionDbContext _context;

    public MatchPredictionRepository(MatchPredictionDbContext context)
    {
        _context = context;
    }

    public async Task Insert(MatchPredictionResult result)
    {
        _context.MatchPredictionResults.Add(result);
        await _context.SaveChangesAsync();
    }

    public async Task Insert(IEnumerable<MatchPredictionResult> results)
    {
        _context.MatchPredictionResults.AddRange(results);
        await _context.SaveChangesAsync();
    }
}