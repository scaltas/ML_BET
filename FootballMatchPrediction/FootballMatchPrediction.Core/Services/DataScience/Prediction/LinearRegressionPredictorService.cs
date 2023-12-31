﻿using Accord.Statistics.Models.Regression.Linear;
using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.DataScience.Prediction;

public class LinearRegressionPredictorService : IPredictorService
{
    public double Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
    {
        double[] inputs = preprocessedMatches.Select(match => match.Odds).ToArray();
        double[] outputs = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.Score.Item1 : match.Score.Item2)).ToArray();

        var ols = new OrdinaryLeastSquares();

        SimpleLinearRegression regression = ols.Learn(inputs, outputs);

        double y = regression.Transform(odds);

        return y;
    }

    public double PredictFirstHalf(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
    {
        double[] inputs = preprocessedMatches.Select(match => match.Odds).ToArray();
        double[] outputs = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.FirstHalfScore.Item1 : match.FirstHalfScore.Item2)).ToArray();

        var ols = new OrdinaryLeastSquares();

        SimpleLinearRegression regression = ols.Learn(inputs, outputs);

        double y = regression.Transform(odds);

        return y;
    }
}
