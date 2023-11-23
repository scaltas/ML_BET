using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.DataScience.Prediction;
using FootballMatchPrediction.Core.Services.DataScience.PreProcessing;
using FootballMatchPrediction.Core.Services.Parse;

namespace FootballMatchPrediction.Core.Services.MatchPrediction;

public class MatchPredictionService : IMatchPredictionService
{
    private readonly IMatchDataService _matchDataService;
    private readonly IPreProcessorService _preProcessorService;
    private readonly IPredictorServiceFactory _predictorServiceFactory;

    public MatchPredictionService(IMatchDataService matchDataService, IPreProcessorService preProcessorService, IPredictorServiceFactory predictorServiceFactory)
    {
        _matchDataService = matchDataService;
        _preProcessorService = preProcessorService;
        _predictorServiceFactory = predictorServiceFactory;
    }

    public async Task<MatchPredictionResult> PredictMatchOutcome(MatchInputModel input)
    {
        var teamUrls = await _matchDataService.GetTeamUrls(input.Match);
        if (teamUrls == null)
            return new MatchPredictionResult() { IsFailed = true };
        

        var oddsScraper = new OddsScraper();
        var oddsData = await oddsScraper.ExtractOddsDataFromUrl(input.Match);

        var homeTeam = teamUrls[0].Split("/")[5];
        var awayTeam = teamUrls[1].Split("/")[5];

        var homeMatchData = await GetMatchData(teamUrls[0], homeTeam, input.SampleCount);
        var awayMatchData = await GetMatchData(teamUrls[1], awayTeam, input.SampleCount);

        var preProcessedHomeData = _preProcessorService.PreProcess(homeTeam, homeMatchData, true);
        var preProcessedAwayData = _preProcessorService.PreProcess(awayTeam, awayMatchData, false);

        var preprocessedMatches = preProcessedHomeData.Concat(preProcessedAwayData)
            .OrderByDescending(d => d.DateTime)
            .Take(input.SampleCount)
            .ToList();

        if (!preprocessedMatches.Any())
            return new MatchPredictionResult() { IsFailed = true };

        var predictorService = _predictorServiceFactory.CreatePredictorService(input.Algorithm);
             
        var result1 = predictorService.Predict(preprocessedMatches, oddsData.Odds1, true);
        var result2 = predictorService.Predict(preprocessedMatches, oddsData.Odds1, false);
        var result = $"{result1:0.00} - {result2:0.00}";

        var firstHalfResult1 = predictorService.PredictFirstHalf(preprocessedMatches, oddsData.Odds1, true);
        var firstHalfResult2 = predictorService.PredictFirstHalf(preprocessedMatches, oddsData.Odds1, false);
        var firstHalfResult = $"{firstHalfResult1:0.00} - {firstHalfResult2:0.00}";

        return new MatchPredictionResult
        {
            Prediction = result,
            FirstHalfPrediction = firstHalfResult,
            Matches = homeMatchData
                .Concat(awayMatchData)
                .OrderByDescending(m => m.Date)
                .Take(input.SampleCount)
                .ToList(),
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            MatchDate = DateTime.Now,
            ActualScore = ""
        };
    }

    private async Task<List<ParsedMatch>> GetMatchData(string url, string teamName, int sampleCount)
    {
        var matches = await _matchDataService.ScrapeMatchData(url, teamName);

        var recentMatches = matches
            .OrderByDescending(m => m.Date)
            .Take(sampleCount)
            .ToList();


        foreach (var match in recentMatches)
        {
            var oddsData = new OddsData();
            if (!string.IsNullOrWhiteSpace(match.MatchUrl))
            {
                var oddsScraper = new OddsScraper();
                oddsData = await oddsScraper.ExtractOddsDataFromUrl(match.MatchUrl);
            }

            if (oddsData.Odds1 == 0)
                continue;

            match.OddsData = oddsData;
        }

        return recentMatches
            .Where(m => m.OddsData != null)
            .ToList();
    }
}