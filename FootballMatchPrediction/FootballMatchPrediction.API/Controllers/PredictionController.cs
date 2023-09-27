using Accord.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
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
            const int numberOfSeasonsToRetrieve = 1;

            var matchData = _matchDataService.ScrapeMatchData(input.TeamId, input.TeamName, numberOfSeasonsToRetrieve);

            var prediction = MakePrediction(input, matchData);

            return Ok(new
            {
                Prediction = prediction,
                HomeTeamMatchData = matchData.OrderByDescending(d => d.Date)
            });
        }

        private object MakePrediction(MatchInputModel input, List<ParsedMatch> matchData)
        {
            var preprocessedMatches = new List<PreprocessedMatch>();
            foreach (var match in matchData)
            {
                var teams = match.Match.Split(" - ");
                var score = match.Score.Split(" - ");
                var reverse = ReplaceTurkishChars(teams[1]) == input.TeamName;
                preprocessedMatches.Add(new PreprocessedMatch()
                {
                    Odds = reverse ? match.OddsData.Odds2 : match.OddsData.Odds1,
                    Score = reverse 
                        ? new Tuple<int, int>(Convert.ToInt32(score[1]), Convert.ToInt32(score[0])) 
                        : new Tuple<int, int>(Convert.ToInt32(score[0]), Convert.ToInt32(score[1]))
                });
            }

            var result1 = Predict(preprocessedMatches, input.Odds1, true);
            var result2 = Predict(preprocessedMatches, input.Odds1, false);
            return $"{result1} - {result2}";
        }

        private object Predict(List<PreprocessedMatch> preprocessedMatches, double odds, bool scored)
        {
            double[] inputs = preprocessedMatches.Select(match => match.Odds).ToArray();
            double[] outputs = preprocessedMatches.Select(match => Convert.ToDouble(scored ? match.Score.Item1 : match.Score.Item2)).ToArray();

            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            SimpleLinearRegression regression = ols.Learn(inputs, outputs);

            double y = regression.Transform(odds); 

            double s = regression.Slope;     
            double c = regression.Intercept; 

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