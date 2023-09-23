using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;
using static System.Net.WebRequestMethods;

namespace FootballMatchPrediction.API.Services
{
    public class MatchDataService : IMatchDataService
    {
        public List<ParsedMatch> ScrapeMatchData(int teamId, string teamName, int numberOfSeasonsToRetrieve)
        {
            var currentSeason = "2023/2024";
            var currentSeasonParts = currentSeason.Split('/');
            var currentStartYear = int.Parse(currentSeasonParts[0]);
            var currentEndYear = int.Parse(currentSeasonParts[1]);

            var matchData = new List<ParsedMatch>();

            for (int i = 0; i < numberOfSeasonsToRetrieve; i++)
            {
                var startYear = currentStartYear - i;
                var endYear = currentEndYear - i;

                var season = $"{startYear}/{endYear}";

                var web = new HtmlWeb();
                var url = $"https://arsiv.mackolik.com/Team/Default.aspx?id={teamId}&season={season}";
                var doc = web.Load(url);

                matchData.AddRange(ParseDoc(doc, teamName));
            }

            return matchData;
        }

        private List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
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
                            if (score == "v")
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

                                if(oddsData.Odds1 == 0 || oddsData.Odds2_5Over == 0)
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
}