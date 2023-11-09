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
                        if (score == "v" || score == "P - P" || score == "0-0")
                            continue;

                        score = score.Replace("-", " - ");

                        var firstHalfScoreArr = row.ChildNodes[21].InnerText.Split();
                        if (firstHalfScoreArr.Length != 2)
                            continue;

                        var firstHalfScore = firstHalfScoreArr[1];

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

                        if(oddsPageUrl != null )

                            matches.Add(new ParsedMatch()
                            {
                                Match = match,
                                Score = score,
                                FirstHalfScore = firstHalfScore,
                                Date = matchDate,
                                MatchUrl = oddsPageUrl
                            });
                    }
                }
            }
        }

        return matches;
    }
}