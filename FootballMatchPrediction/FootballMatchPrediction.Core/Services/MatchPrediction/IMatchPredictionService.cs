using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.MatchPrediction
{
    public interface IMatchPredictionService
    {
        MatchPredictionResult PredictMatchOutcome(MatchInputModel input);
    }

}
