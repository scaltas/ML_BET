using Accord.Statistics.Models.Regression.Linear;
using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.DataScience.Prediction;

public class PredictorService : IPredictorService
{
    public double Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
    {
        return PredictWithLinearRegression(preprocessedMatches, odds, scored);
    }

    private double PredictWithLinearRegression(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
    {
        double[] inputs = preprocessedMatches.Select(match => match.Odds).ToArray();
        double[] outputs = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.Score.Item1 : match.Score.Item2)).ToArray();

        var ols = new OrdinaryLeastSquares();

        SimpleLinearRegression regression = ols.Learn(inputs, outputs);

        double y = regression.Transform(odds);

        return y;
    }

    private double PredictWithTimeSeriesAnalysis(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
    {
        double[] pastScores = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.Score.Item1 : match.Score.Item2)).ToArray();

        double[] timeFeatures = preprocessedMatches.Select(match => (DateTime.Now - match.DateTime).TotalDays).ToArray();

        double[] pastOdds = preprocessedMatches.Select(match => match.Odds).ToArray();

        var forecastedScore = 0;

        //Implement a timeseries algorithm.

        return forecastedScore;
    }
}
