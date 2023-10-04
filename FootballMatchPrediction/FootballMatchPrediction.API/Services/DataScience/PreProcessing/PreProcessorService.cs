using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services.DataScience.PreProcessing;

public class PreProcessorService : IPreProcessorService
{
    public List<PreprocessedMatch> PreProcess(string teamName, List<ParsedMatch> matchData, bool isHome)
    {
        var preprocessedMatches = new List<PreprocessedMatch>();
        foreach (var match in matchData)
        {
            var teams = match.Match.Split(" - ");
            var score = match.Score.Split(" - ");
            var reverse = ReplaceTurkishChars(teams[1]) == RemoveDashes(teamName);
            reverse = isHome ? reverse : !reverse;

            preprocessedMatches.Add(new PreprocessedMatch()
            {
                Odds = reverse ? match.OddsData.Odds2 : match.OddsData.Odds1,
                Score = reverse
                    ? new Tuple<int, int>(Convert.ToInt32(score[1]), Convert.ToInt32(score[0]))
                    : new Tuple<int, int>(Convert.ToInt32(score[0]), Convert.ToInt32(score[1]))
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