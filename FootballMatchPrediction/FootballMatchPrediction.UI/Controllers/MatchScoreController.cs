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
        string url = "https://arsiv.mackolik.com/Program/Program.aspx";
        var numbers = await GetNumbersFromWebsite(url);

        var viewModel = new List<MatchScore>();

        foreach (string number in numbers)
        {
            var result = _predictionService.PredictMatchOutcome(new MatchInputModel()
            {
                Match = $"https://arsiv.mackolik.com/Match/Default.aspx?id={number}"
            });
            
            viewModel.Add(new MatchScore()
            {
                MatchId = Convert.ToInt32(number),
                HomeTeam = result.HomeTeam,
                AwayTeam = result.AwayTeam,
                PredictedScore = result.Prediction,
                ActualScore = result.ActualScore,
                MatchDate = result.MatchDate
            });
        }

        return View(viewModel);

    }

    static async Task<string[]> GetNumbersFromWebsite(string url)
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

    static string ExtractNumber(string input)
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