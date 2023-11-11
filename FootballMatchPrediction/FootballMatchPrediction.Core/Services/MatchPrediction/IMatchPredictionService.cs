using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.MatchPrediction
{
    public interface IMatchPredictionService
    {
        Task<MatchPredictionResult> PredictMatchOutcome(MatchInputModel input);
    }

}
