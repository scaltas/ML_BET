using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.UI.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace FootballMatchPrediction.UI.Controllers;

public class MatchScoreController : Controller
{
    private readonly IMatchPredictionService _predictionService;
    public MatchScoreController(IMatchPredictionService predictionService)
    {
        _predictionService = predictionService;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }
    public async Task<IActionResult> GetMatchPrediction(string number)
    {
        var result = _predictionService.PredictMatchOutcome(new MatchInputModel()
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
            MatchDate = result.MatchDate
        };

        return Json(viewModel);
    }

    public async Task<IActionResult> GetMatchNumbers()
    {
        string url = "https://arsiv.mackolik.com/Program/Program.aspx";
        var numbers = await GetNumbersFromWebsite(url);
        return Json(numbers);
    }

    private async Task<string[]> GetNumbersFromWebsite(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            string htmlContent = await client.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@href, 'popMatch')]");
            if (linkNodes == null)
            {
                return Array.Empty<string>();
            }

            return linkNodes.Select(node => ExtractNumber(node.GetAttributeValue("href", ""))).ToArray();
        }
    }
    private string ExtractNumber(string input)
    {
        int startIndex = input.IndexOf("popMatch(");
        int endIndex = input.IndexOf(",\"ByLeague\"", startIndex);

        if (startIndex != -1 && endIndex != -1)
        {
            startIndex += "popMatch(".Length;
            return input.Substring(startIndex, endIndex - startIndex);
        }

        return string.Empty;
    }
}