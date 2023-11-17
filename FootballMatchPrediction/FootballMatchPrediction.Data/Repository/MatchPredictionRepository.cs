using FootballMatchPrediction.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace FootballMatchPrediction.Data.Repository;

public class MatchPredictionRepository : IMatchPredictionRepository
{
    private readonly MatchPredictionDbContext _context;

    public MatchPredictionRepository(MatchPredictionDbContext context)
    {
        _context = context;
    }

    public IQueryable<MatchPredictionResult> GetAllPredictions()
    {
        return _context.MatchPredictionResults;
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

    public async Task Update(MatchPredictionResult result)
    {
        _context.Update(result);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAll()
    {
        var allResults = await _context.MatchPredictionResults.ToListAsync();
        _context.MatchPredictionResults.RemoveRange(allResults);
        await _context.SaveChangesAsync();
    }
}