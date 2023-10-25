using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.DataScience.Prediction
{
    public interface IPredictorService
    {
        double Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored);
        double PredictFirstHalf(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored);
    }
}
