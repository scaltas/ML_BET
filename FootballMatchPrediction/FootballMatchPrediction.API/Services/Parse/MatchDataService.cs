﻿using Accord.IO;
using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace FootballMatchPrediction.API.Services.Parse
{
    public class MatchDataService : IMatchDataService
    {
        public List<ParsedMatch> ScrapeMatchData(string teamUrl, string teamName)
        {
            var matchData = new List<ParsedMatch>();

            var web = new HtmlWeb();
            
            if (!teamUrl.Contains("https"))
                teamUrl = $"https:{teamUrl}";
            
            var doc = web.Load(teamUrl);

            matchData.AddRange(ParseDoc(doc, teamName));

            return matchData;
        }

        public string[] GetTeamUrls(string matchUrl)
        {
            var result = new List<string>();

            var web = new HtmlWeb();
            var doc = web.Load(matchUrl);

            {
                var element = doc.DocumentNode.SelectSingleNode("//a[@class='left-block-team-name']");
                string hrefValue = element.GetAttributeValue("href", "");
                result.Add(hrefValue);
            }

            {
                var element = doc.DocumentNode.SelectSingleNode("//a[@class='r-left-block-team-name']");
                string hrefValue = element.GetAttributeValue("href", "");
                result.Add(hrefValue);
            }
            

            return result.ToArray();
        }

        private List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
        {
            var isBasketball = doc.Text.Contains("Basketball");

            var rows = 
                isBasketball ?
                    doc.DocumentNode.SelectNodes("//tr[contains(@class, 'alt')]"):
                    doc.DocumentNode.SelectNodes("//tr[contains(@class, 'row')]");
            
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
                            if (isBasketball)
                                score = score.Replace("-", " - ");

                            var match = "";
                            if (isBasketball)
                            {
                                string[] lines = row.InnerText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                if (lines.Length > 0)
                                {
                                    match = $"{lines[7].Trim()} - {lines[15].Trim()}";
                                }
                            }
                            else
                            {
                                match = scoreElement
                                    .SelectNodes(".//meta[contains(@itemprop, 'name')]")?
                                    .Select(meta => meta.GetAttributeValue("content", ""))?
                                    .FirstOrDefault();
                            }

                            var dateMeta = "";
                            if (isBasketball)
                            {
                                string[] lines = row.InnerText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                                if (lines.Length > 0)
                                {
                                    dateMeta = lines[1].Trim();
                                }
                            }
                            else
                                dateMeta = scoreElement
                                    .SelectNodes(".//meta[contains(@itemprop, 'startDate')]")?
                                    .Select(meta => meta.GetAttributeValue("content", ""))?
                                    .FirstOrDefault();

                            if (match != null || isBasketball)
                            {
                                DateTime? matchDate = null;
                                if (!string.IsNullOrWhiteSpace(dateMeta))
                                {
                                    if (isBasketball)
                                    {
                                        string[] formats = { "dd.MM.yyyy", "d.MM.yyyy" };

                                        if (DateTime.TryParseExact(dateMeta, formats, CultureInfo.InvariantCulture,
                                                DateTimeStyles.None, out DateTime parsedDate))
                                        {
                                            matchDate = parsedDate;
                                        }
                                    }
                                    else
                                    {
                                        if (DateTime.TryParse(dateMeta, out DateTime parsedDate))
                                        {
                                            matchDate = parsedDate;
                                        }
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

                                if(oddsData.Odds1 == 0)
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