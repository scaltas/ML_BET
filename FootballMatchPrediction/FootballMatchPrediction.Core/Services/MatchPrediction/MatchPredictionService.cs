using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.DataScience.Prediction;
using FootballMatchPrediction.Core.Services.DataScience.PreProcessing;
using FootballMatchPrediction.Core.Services.Parse;

namespace FootballMatchPrediction.Core.Services.MatchPrediction;

public class MatchPredictionService : IMatchPredictionService
{
    private readonly IMatchDataService _matchDataService;
    private readonly IPreProcessorService _preProcessorService;
    private readonly IPredictorService _predictorService;

    public MatchPredictionService(IMatchDataService matchDataService, IPreProcessorService preProcessorService, IPredictorService predictorService)
    {
        _matchDataService = matchDataService;
        _preProcessorService = preProcessorService; 
        _predictorService = predictorService;
    }

    public MatchPredictionResult PredictMatchOutcome(MatchInputModel input)
    {
        var teamUrls = _matchDataService.GetTeamUrls(input.Match);

        var oddsScraper = new OddsScraper();
        var oddsData = oddsScraper.ExtractOddsDataFromUrl(input.Match);

        var homeTeam = teamUrls[0].Split("/")[5];
        var awayTeam = teamUrls[1].Split("/")[5];

        var homeMatchData = GetMatchData(teamUrls[0], homeTeam);
        var awayMatchData = GetMatchData(teamUrls[1], awayTeam);

        var preProcessedHomeData = _preProcessorService.PreProcess(homeTeam, homeMatchData, true);
        var preProcessedAwayData = _preProcessorService.PreProcess(awayTeam, awayMatchData, false);

        var preprocessedMatches = preProcessedHomeData.Concat(preProcessedAwayData)
            .OrderByDescending(d => d.DateTime)
            .Take(10)
            .ToList();

        var result1 = _predictorService.Predict(preprocessedMatches, oddsData.Odds1, true);
        var result2 = _predictorService.Predict(preprocessedMatches, oddsData.Odds1, false);
        var result = $"{result1} - {result2}";

        var firstHalfResult1 = _predictorService.PredictFirstHalf(preprocessedMatches, oddsData.Odds1, true);
        var firstHalfResult2 = _predictorService.PredictFirstHalf(preprocessedMatches, oddsData.Odds1, false);
        var firstHalfResult = $"{firstHalfResult1} - {firstHalfResult2}";

        return new MatchPredictionResult
        {
            Prediction = result,
            FirstHalfPrediction = firstHalfResult,
            Matches = homeMatchData
                .Concat(awayMatchData)
                .OrderByDescending(m => m.Date)
                .Take(10)
                .ToList(),
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            MatchDate = DateTime.Now,
            ActualScore = ""
        };
    }

    private List<ParsedMatch> GetMatchData(string url, string teamName)
    {
        return _matchDataService.ScrapeMatchData(url, teamName);
    }
}