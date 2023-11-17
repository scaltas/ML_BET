using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using Microsoft.AspNetCore.Mvc;

namespace FootballMatchPrediction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchPredictionController : ControllerBase
    {
        private readonly IMatchPredictionService _predictionService;

        public MatchPredictionController(IMatchPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost]
        public async Task<IActionResult> PredictMatchOutcome(MatchInputModel input)
        {
            var predictionResult = await _predictionService.PredictMatchOutcome(input);
            return Ok(predictionResult);
        }
    }
}