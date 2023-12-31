﻿using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.DataScience.PreProcessing;

public class PreProcessorService : IPreProcessorService
{
    public List<PreprocessedMatch> PreProcess(string teamName, List<ParsedMatch> matchData, bool isHome)
    {
        var preprocessedMatches = new List<PreprocessedMatch>();
        foreach (var match in matchData)
        {
            var teams = match.Match.Split(" - ");
            var score = match.Score.Split(" - ");
            var firstHalfScore = match.FirstHalfScore.Split("-");
            var reverse = ReplaceTurkishChars(teams[1]) == RemoveDashes(teamName);
            reverse = isHome ? reverse : !reverse;

            preprocessedMatches.Add(new PreprocessedMatch()
            {
                Odds = reverse ? match.OddsData.Odds2 : match.OddsData.Odds1,
                Score = reverse
                    ? new Tuple<int, int>(Convert.ToInt32(score[1]), Convert.ToInt32(score[0]))
                    : new Tuple<int, int>(Convert.ToInt32(score[0]), Convert.ToInt32(score[1])),
                FirstHalfScore = reverse
                    ? new Tuple<int, int>(Convert.ToInt32(firstHalfScore[1]), Convert.ToInt32(firstHalfScore[0]))
                    : new Tuple<int, int>(Convert.ToInt32(firstHalfScore[0]), Convert.ToInt32(firstHalfScore[1])),
                DateTime = match.Date.GetValueOrDefault()
            });
        }

        return preprocessedMatches;
    }

    private string ReplaceTurkishChars(string input)
    {
        return input
            .Replace("ş", "s")
            .Replace("ç", "c");
    }

    private string RemoveDashes(string input)
    {
        return input
            .Replace("-", " ");
    }
}