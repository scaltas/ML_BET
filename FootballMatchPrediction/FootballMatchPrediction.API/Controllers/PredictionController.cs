using FootballMatchPrediction.API.Models;
using FootballMatchPrediction.API.Services;
using FootballMatchPrediction.API.Services.DataScience.Prediction;
using FootballMatchPrediction.API.Services.DataScience.PreProcessing;
using FootballMatchPrediction.API.Services.Parse;
using Microsoft.AspNetCore.Mvc;


namespace FootballMatchPrediction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchPredictionController : ControllerBase
    {
        private readonly IMatchDataService _matchDataService;
        private readonly IPreProcessorService _preProcessorService;
        private readonly IPredictorService _predictorService;

        public MatchPredictionController(IMatchDataService matchDataService, IPreProcessorService preProcessorService, IPredictorService predictorService)
        {
            _matchDataService = matchDataService;
            _preProcessorService = preProcessorService;
            _predictorService = predictorService;
        }

        [HttpPost]
        public IActionResult PredictMatchOutcome(MatchInputModel input)
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

            return Ok(new
            {
                Prediction = result,
                Matches = homeMatchData
                    .Concat(awayMatchData)
                    .OrderByDescending(m => m.Date)
                    .Take(10)
                    .ToList()
            });
        }

        private List<ParsedMatch> GetMatchData(string url, string teamName)
        {
            return _matchDataService
                .ScrapeMatchData(url, teamName);
        }

    }
}