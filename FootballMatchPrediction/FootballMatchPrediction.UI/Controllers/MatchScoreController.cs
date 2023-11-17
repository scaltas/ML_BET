using FootballMatchPrediction.Data.Repository;
using FootballMatchPrediction.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FootballMatchPrediction.Data.Model;

namespace FootballMatchPrediction.UI.Controllers;

public class MatchScoreController : Controller
{
    private readonly IMatchPredictionRepository _predictionRepository;

    public MatchScoreController(IMatchPredictionRepository predictionRepository)
    {
        _predictionRepository = predictionRepository;
    }

    public async Task<IActionResult> Index(DateTime? selectedDate)
    {
        var currentDate = selectedDate ?? DateTime.Now.Date;
        var predictions = await _predictionRepository.GetAllPredictions()
            .Where(p => p.MatchDate.Date == currentDate.Date)
            .ToListAsync();
        
        var viewModel = predictions.Select(result => new MatchScore
        {
            MatchId = result.Id,
            HomeTeam = result.HomeTeam,
            AwayTeam = result.AwayTeam,
            FirstHalfScore = result.FirstHalfPrediction,
            PredictedScore = result.Prediction,
            ActualScore = result.ActualScore ?? "-",
            MatchDate = result.MatchDate,
            ViewOrder = result.ViewOrder,
            Success = CheckSuccess(result)
        })
            .OrderBy(m => m.ViewOrder)
            .ToList();

        ViewBag.SelectedDate = currentDate;

        return View(viewModel);
    }

    private bool CheckSuccess(MatchPredictionResult result)
    {
        return false;
    }
}