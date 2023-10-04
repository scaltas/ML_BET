using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services.DataScience.Prediction
{
    public interface IPredictorService
    {
        double Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored);
    }
}
