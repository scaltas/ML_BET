﻿using FootballMatchPrediction.Data.Repository;
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

    public async Task<IActionResult> Index(DateTime? selectedDate)
    {
        var currentDate = selectedDate ?? DateTime.Now.Date;
        var predictions = await _predictionRepository.GetAllPredictions();
        predictions = predictions.Where(p => p.MatchDate.Date == currentDate.Date);

        if (selectedDate.HasValue)
        {
            predictions = predictions.Where(p => p.MatchDate.Date == selectedDate.Value.Date);
        }
        
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

        ViewBag.SelectedDate = currentDate;

        return View(viewModel);
    }
}