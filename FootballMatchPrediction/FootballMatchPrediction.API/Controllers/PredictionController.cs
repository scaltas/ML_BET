using Accord.Statistics.Models.Regression.Linear;
using FootballMatchPrediction.API.Models;
using FootballMatchPrediction.API.Services;
using Microsoft.AspNetCore.Mvc;


namespace FootballMatchPrediction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchPredictionController : ControllerBase
    {
        private readonly IMatchDataService _matchDataService;

        public MatchPredictionController(IMatchDataService matchDataService)
        {
            _matchDataService = matchDataService;
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

            var predictionForHome = MakePrediction(homeTeam, oddsData.Odds1, homeMatchData, true);
            var predictionForAway = MakePrediction(awayTeam, oddsData.Odds2, awayMatchData, false);

            return Ok(new
            {
                PredictionForHome = predictionForHome,
                PredictionForAway = predictionForAway,
                HomeMatches = homeMatchData,
                AwayMatches = awayMatchData
            });
        }

        private List<ParsedMatch> GetMatchData(string url, string teamName)
        {
            return _matchDataService
                .ScrapeMatchData(url, teamName)
                .OrderByDescending(item => item.Date)
                .Take(10)
                .ToList();
        }

        private object MakePrediction(string teamName, double odds, List<ParsedMatch> matchData, bool isHome)
        {
            var preprocessedMatches = new List<PreprocessedMatch>();
            foreach (var match in matchData)
            {
                var teams = match.Match.Split(" - ");
                var score = match.Score.Split(" - ");
                var reverse = ReplaceTurkishChars(teams[1]) == teamName;
                preprocessedMatches.Add(new PreprocessedMatch()
                {
                    Odds = reverse ? match.OddsData.Odds2 : match.OddsData.Odds1,
                    Score = reverse 
                        ? new Tuple<int, int>(Convert.ToInt32(score[1]), Convert.ToInt32(score[0])) 
                        : new Tuple<int, int>(Convert.ToInt32(score[0]), Convert.ToInt32(score[1]))
                });
            }

            var result1 = Predict(preprocessedMatches, odds, true);
            var result2 = Predict(preprocessedMatches, odds, false);
            return isHome ? $"{result1} - {result2}" : $"{result2} - {result1}";
        }

        private object Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
        {
            double[] inputs = preprocessedMatches.Select(match => match.Odds).ToArray();
            double[] outputs = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.Score.Item1 : match.Score.Item2)).ToArray();

            var ols = new OrdinaryLeastSquares();

            SimpleLinearRegression regression = ols.Learn(inputs, outputs);

            double y = regression.Transform(odds); 

            return y;
        }

        private string ReplaceTurkishChars(string input)
        {
            return input
                .Replace("þ", "s")
                .Replace("ç", "c");
        }
    }
}