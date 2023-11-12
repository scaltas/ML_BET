using FootballMatchPrediction.Data.Model;

namespace FootballMatchPrediction.Data.Repository;

public interface IMatchPredictionRepository
{
    Task<IEnumerable<MatchPredictionResult>> GetAllPredictions();
    Task Insert(MatchPredictionResult result);
    Task Insert(IEnumerable<MatchPredictionResult> results);
    Task DeleteAll();
}