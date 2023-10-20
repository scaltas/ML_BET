using FootballMatchPrediction.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class MatchScoreController : Controller
{
    private readonly HttpClient _httpClient;

    public MatchScoreController()
    {
        _httpClient = new HttpClient();
        // Configure the base URL for your backend API.
        _httpClient.BaseAddress = new Uri("https://your-backend-api-url.com/");
    }

    public async Task<IActionResult> Index()
    {
        return View(GetMockMatchScores());

        // Fetch match scores from the backend API.
        /*var response = await _httpClient.GetAsync("api/matchscores");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var matchScores = JsonSerializer.Deserialize<List<MatchScore>>(content);
            return View(matchScores);
        }
        else
        {
            // Handle the error
            return View("Error");
        }*/
    }

    private List<MatchScore> GetMockMatchScores()
    {
        var mockMatchScores = new List<MatchScore>
        {
            new MatchScore
            {
                MatchId = 1,
                HomeTeam = "Team A",
                AwayTeam = "Team B",
                PredictedScore = "2 - 1", // Successful prediction
                ActualScore = "2 - 1",    // Actual score matches prediction
                MatchDate = new DateTime(2023, 10, 15)
            },
            new MatchScore
            {
                MatchId = 2,
                HomeTeam = "Team C",
                AwayTeam = "Team D",
                PredictedScore = "0 - 2",
                ActualScore = "2 - 2",    // Actual score does not match prediction
                MatchDate = new DateTime(2023, 10, 17)
            },
            // Add more sample match scores here
        };

        return mockMatchScores;
    }
}