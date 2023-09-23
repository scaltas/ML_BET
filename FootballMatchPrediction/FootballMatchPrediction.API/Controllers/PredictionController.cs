using FootballMatchPrediction.API.Models;
using FootballMatchPrediction.API.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

            var homeTeamMatchData = _matchDataService.ScrapeMatchData(input.HomeTeamId, input.HomeTeamName, numberOfSeasonsToRetrieve);
            var awayTeamMatchData = _matchDataService.ScrapeMatchData(input.AwayTeamId, input.AwayTeamName, numberOfSeasonsToRetrieve);

            var prediction = MakePrediction(homeTeamMatchData, awayTeamMatchData);

            return Ok(new
            {
                Prediction = prediction,
                HomeTeamMatchData = homeTeamMatchData.OrderByDescending(d => d.Date),
                AwayTeamMatchData = awayTeamMatchData.OrderByDescending(d => d.Date),
            });
        }

        private string MakePrediction(List<ParsedMatch> homeTeamMatchData, List<ParsedMatch> awayTeamMatchData)
        {
            // TODO Replace this with actual prediction logic.
            return "Home Team Wins";
        }
    }
}