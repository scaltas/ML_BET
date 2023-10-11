using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;
using System.Globalization;

namespace FootballMatchPrediction.API.Services.Parse;

public class FootballParser : IParser
{
    public List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
    {
        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'row')]");

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

                        var match = scoreElement
                            .SelectNodes(".//meta[contains(@itemprop, 'name')]")?
                            .Select(meta => meta.GetAttributeValue("content", ""))?
                            .FirstOrDefault();

                        var dateMeta = scoreElement
                                .SelectNodes(".//meta[contains(@itemprop, 'startDate')]")?
                                .Select(meta => meta.GetAttributeValue("content", ""))?
                                .FirstOrDefault();

                        if (match != null)
                        {
                            DateTime? matchDate = null;
                            if (!string.IsNullOrWhiteSpace(dateMeta))
                            {

                                if (DateTime.TryParse(dateMeta, out DateTime parsedDate))
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
        }

        return matches;
    }
}