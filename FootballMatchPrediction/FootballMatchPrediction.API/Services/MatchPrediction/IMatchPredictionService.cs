using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services.MatchPrediction
{
    public interface IMatchPredictionService
    {
        MatchPredictionResult PredictMatchOutcome(MatchInputModel input);
    }

}
