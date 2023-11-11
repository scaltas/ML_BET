using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballMatchPrediction.UI.Controllers;

public class MatchScoreController : Controller
{
    private readonly IMatchPredictionService _predictionService;
    private readonly IMatchDataService _matchDataService;
    public MatchScoreController(IMatchPredictionService predictionService, IMatchDataService matchDataService)
    {
        _predictionService = predictionService;
        _matchDataService = matchDataService;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }
    public async Task<IActionResult> GetMatchPredictions(string number)
    {
        var result = await _predictionService.PredictMatchOutcome(new MatchInputModel()
        {
            Match = $"https://arsiv.mackolik.com/Match/Default.aspx?id={number}"
        });

        var viewModel = new MatchScore()
        {
            MatchId = Convert.ToInt32(number),
            HomeTeam = result.HomeTeam,
            AwayTeam = result.AwayTeam,
            FirstHalfScore = result.FirstHalfPrediction,
            PredictedScore = result.Prediction,
            MatchDate = result.MatchDate,
            SampleCount = result.Matches.Count
        };

        return Json(viewModel);
    }

    public async Task<IActionResult> GetMatchNumbers()
    {
        var numbers = await _matchDataService.GetMatchIdsFromWebsite();
        return Json(numbers);
    }


}