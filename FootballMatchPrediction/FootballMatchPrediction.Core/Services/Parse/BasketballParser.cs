using System.Globalization;
using FootballMatchPrediction.Core.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.Core.Services.Parse;

public class BasketballParser : IParser
{
    public List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
    {
        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'alt')]");
            
        var xpathExpression = $".//a[contains(@href, '{teamName}')]";

        var matches = new List<ParsedMatch>();

        if (rows != null)
        {
            foreach (var row in rows)
            {
                var teamLinks = row.SelectNodes(xpathExpression);

                if (teamLinks != null)
                {
                    var scoreElement = teamLinks.FirstOrDefault();
                    if (scoreElement != null)
                    {
                        var score = scoreElement.InnerText.Trim();
                        if (score == "v" || score == "P - P")
                            continue;

                        score = score.Replace("-", " - ");

                        var match = "";
                        {
                            string[] lines = row.InnerText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lines.Length > 0)
                            {
                                match = $"{lines[7].Trim()} - {lines[15].Trim()}";
                            }
                        }

                        var dateMeta = "";
                        {
                            string[] lines = row.InnerText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lines.Length > 0)
                            {
                                dateMeta = lines[1].Trim();
                            }
                        }

                        DateTime? matchDate = null;
                        if (!string.IsNullOrWhiteSpace(dateMeta))
                        {
                            string[] formats = { "dd.MM.yyyy", "d.MM.yyyy" };

                            if (DateTime.TryParseExact(dateMeta, formats, CultureInfo.InvariantCulture,
                                    DateTimeStyles.None, out DateTime parsedDate))
                            {
                                matchDate = parsedDate;
                            }
                        }

                        // Extract and parse the odds data by visiting the odds page URL
                        var oddsPageUrl = scoreElement.Attributes["href"]?.Value;

                        var oddsData = new OddsData();
                        if (!string.IsNullOrWhiteSpace(oddsPageUrl))
                        {
                            var oddsScraper = new OddsScraper();
                            oddsData = oddsScraper.ExtractOddsDataFromUrl(oddsPageUrl);
                        }

                        if (oddsData.Odds1 == 0)
                            continue;

                        matches.Add(new ParsedMatch()
                        {
                            Match = match,
                            Score = score,
                            Date = matchDate,
                            OddsData = oddsData
                        });
                    }
                }
            }
        }

        return matches;
    }
}