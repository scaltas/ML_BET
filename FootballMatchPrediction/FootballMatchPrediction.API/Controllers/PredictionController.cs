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

            var preProcessedHomeData = PreProcess(homeTeam, homeMatchData, true);
            var preProcessedAwayData = PreProcess(awayTeam, awayMatchData, false);

            var preprocessedMatches = preProcessedHomeData.Concat(preProcessedAwayData).ToList();

            var result1 = Predict(preprocessedMatches, oddsData.Odds1, true);
            var result2 = Predict(preprocessedMatches, oddsData.Odds1, false);
            var result = $"{result1} - {result2}";

            return Ok(new
            {
                Prediction = result,
                HomeMatches = homeMatchData,
                AwayMatches = awayMatchData
            });
        }

        private List<ParsedMatch> GetMatchData(string url, string teamName)
        {
            return _matchDataService
                .ScrapeMatchData(url, teamName)
                .Where(item => item.Date > DateTime.Now.AddMonths(-1))
                .ToList();
        }

        private List<PreprocessedMatch> PreProcess(string teamName,List<ParsedMatch> matchData, bool isHome)
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

        private string RemoveDashes(string input)
        {
            return input
                .Replace("-", " ");
        }
    }
}