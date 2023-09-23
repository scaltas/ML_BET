using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace FootballMatchPrediction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchPredictionController : ControllerBase
    {
        [HttpPost]
        public IActionResult PredictMatchOutcome(MatchInputModel input)
        {
            const int numberOfSeasonsToRetrieve = 10;

            var homeTeamMatchData = new List<ParsedMatch>();
            var awayTeamMatchData = new List<ParsedMatch>();

            var currentSeason = "2023/2024";

            var currentSeasonParts = currentSeason.Split('/');
            var currentStartYear = int.Parse(currentSeasonParts[0]);
            var currentEndYear = int.Parse(currentSeasonParts[1]);

            for (int i = 0; i < numberOfSeasonsToRetrieve; i++)
            {
                var startYear = currentStartYear - i;
                var endYear = currentEndYear - i;

                var season = $"{startYear}/{endYear}";

                homeTeamMatchData.AddRange(ScrapeMatchData(input.HomeTeamId, input.HomeTeamName, season));
                awayTeamMatchData.AddRange(ScrapeMatchData(input.AwayTeamId, input.AwayTeamName, season));
            }

            var prediction = MakePrediction(homeTeamMatchData, awayTeamMatchData);

            return Ok(prediction);
        }

        private List<ParsedMatch> ScrapeMatchData(int teamId, string teamName, string season)
        {
            var web = new HtmlWeb();

            var url = $"https://arsiv.mackolik.com/Team/Default.aspx?id={teamId}&season={season}";
            var doc = web.Load(url);

            var matchData = ParseDoc(doc, teamName);

            return matchData;
        }

        private List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
        {
            // Select all rows with class 'row' (assuming this selects all the match rows)
            var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'row')]");

            // Construct the XPath expression by concatenating the teamName variable
            string xpathExpression = $".//a[contains(@href, '{teamName}')]";

            var matches = new List<ParsedMatch>();

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var teamLinks = row.SelectNodes(xpathExpression);

                    if (teamLinks != null)
                    {
                        // Extract the score from the row (assuming it's the text of the relevant <a> element)
                        var scoreElement = teamLinks.FirstOrDefault();
                        if (scoreElement != null)
                        {
                            var score = scoreElement.InnerText.Trim();
                            var match = scoreElement
                                .SelectNodes(".//meta[contains(@itemprop, 'name')]")?
                                .Select(meta => meta.GetAttributeValue("content", ""))?
                                .FirstOrDefault();

                            if (match != null)
                            {
                                matches.Add(new ParsedMatch()
                                {
                                    Match = match,
                                    Score = score,
                                    Date = DateTime.Now
                                });

                                //var teams = match.Split(" - ");

                                //scores.Add(ReplaceTurkishCharacters(teams[0]) == teamName ? score : ReverseScore(score));
                            }
                        }
                    }
                }
            }

            return matches;
        }

        private string ReplaceTurkishCharacters(string team)
        {
            return team
                .Replace("þ", "s")
                .Replace("ç", "c");

        }

        private string ReverseScore(string score)
        {
            // Split the score into homeScore and awayScore
            var parts = score.Split('-');

            if (parts.Length == 2)
            {
                // Swap the scores and format them as "Y - X"
                var homeScore = parts[0].Trim();
                var awayScore = parts[1].Trim();
                return $"{awayScore} - {homeScore}";
            }

            // If the score format is not as expected, return it as is
            return score;
        }

        private string MakePrediction(List<ParsedMatch> homeTeamMatchData, List<ParsedMatch> awayTeamMatchData)
        {
            // TODO Replace this with actual prediction logic.
            return "Home Team Wins";
        }
    }
}