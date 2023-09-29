using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace FootballMatchPrediction.API.Services
{
    public class OddsScraper
    {
        public OddsData ExtractOddsDataFromUrl(string oddsPageUrl)
        {
            var web = new HtmlWeb();

            if (!oddsPageUrl.Contains("https"))
                oddsPageUrl = $"https:{oddsPageUrl}";

            var doc = web.Load($"{oddsPageUrl}");

            var oddsData = ExtractOddsValue(doc);

            return oddsData;
        }

        private OddsData ExtractOddsValue(HtmlDocument doc)
        {
            var oddsData = new OddsData();

            // Find the element with text '2,5 Alt/Üst'
            var altUstElement = doc.DocumentNode.SelectSingleNode("//a[@class='compare-rate-bg-up' and contains(@href, '2,5 Alt/Üst')]");

            if (altUstElement != null)
            {
                var href = altUstElement.GetAttributeValue("href", "");
                var oddsValues = Regex.Matches(href, @"'([^']*)'").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

                if (oddsValues.Any())
                {
                    if (double.TryParse(oddsValues[5], out double overOdds))
                    {
                        oddsData.Odds2_5Over = overOdds;
                    }
                }
            }

            // Find the element with text 'Maç Sonucu'
            var macSonucuElement = doc.DocumentNode.SelectSingleNode("//a[@class='compare-rate-bg-up' and contains(@href, 'Maç Sonucu')]");

            if (macSonucuElement != null)
            {
                var href = macSonucuElement.GetAttributeValue("href", "");
                var oddsValues = Regex.Matches(href, @"'([^']*)'").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

                if (oddsValues.Any())
                {
                    if (double.TryParse(oddsValues[5], out double odds1) &&
                        double.TryParse(oddsValues[7], out double odds2))
                    {
                        oddsData.Odds1 = odds1;
                        oddsData.Odds2 = odds2;
                    }
                }
            }

            return oddsData;
        }
    }
}
