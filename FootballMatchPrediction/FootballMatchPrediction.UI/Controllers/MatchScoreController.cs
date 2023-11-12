using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.Data.Repository;
using FootballMatchPrediction.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FootballMatchPrediction.UI.Controllers;

public class MatchScoreController : Controller
{
    private readonly IMatchPredictionRepository _predictionRepository;

    public MatchScoreController(IMatchPredictionRepository predictionRepository)
    {
        _predictionRepository = predictionRepository;
    }

    public async Task<IActionResult> Index()
    {
        var predictions = await _predictionRepository.GetAllPredictions();
        var viewModel = predictions.Select(result => new MatchScore
        {
            MatchId = result.Id,
            HomeTeam = result.HomeTeam,
            AwayTeam = result.AwayTeam,
            FirstHalfScore = result.FirstHalfPrediction,
            PredictedScore = result.Prediction,
            MatchDate = result.MatchDate,
            ViewOrder = result.ViewOrder
        })
            .OrderBy(m => m.ViewOrder)
            .ToList();

        return View(viewModel);
    }
}