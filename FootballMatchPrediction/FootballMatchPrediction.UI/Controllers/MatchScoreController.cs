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

        int totalPredictions = viewModel.Count();
        int successfulPredictions = viewModel.Count(m => m.Success);

        ViewBag.SuccessRate = (double)successfulPredictions / totalPredictions * 100;

        return View(viewModel);
    }

    private bool CheckSuccess(MatchPredictionResult result)
    {
        try
        {
            if (string.IsNullOrEmpty(result.ActualScore) || result.ActualScore == "v")
                return false;

            // Parse the predicted score
            string[] predictedScores = result.Prediction.Split(" - ");
            double predictedTeam1Score = double.Parse(predictedScores[0].Trim());
            double predictedTeam2Score = double.Parse(predictedScores[1].Trim());

            // Parse the actual score
            string[] actualScores = result.ActualScore.Split(" - ");
            int actualTeam1Score = int.Parse(actualScores[0].Trim());
            int actualTeam2Score = int.Parse(actualScores[1].Trim());

            // Check if the difference is smaller than 0.5
            bool isPredictionCorrect =
                Math.Abs(predictedTeam1Score - actualTeam1Score) < 0.5 &&
                Math.Abs(predictedTeam2Score - actualTeam2Score) < 0.5;

            return isPredictionCorrect;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    
}