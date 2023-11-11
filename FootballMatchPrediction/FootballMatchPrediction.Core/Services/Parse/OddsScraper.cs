using System.Text.RegularExpressions;
using FootballMatchPrediction.Core.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.Core.Services.Parse
{
    public class OddsScraper
    {
        public async Task<OddsData> ExtractOddsDataFromUrl(string oddsPageUrl)
        {
            var web = new HtmlWeb();

            if (!oddsPageUrl.Contains("https"))
                oddsPageUrl = $"https:{oddsPageUrl}";

            var doc = await web.LoadFromWebAsync($"{oddsPageUrl}");

            var oddsData = ExtractOddsValue(doc);

            return oddsData;
        }

        private OddsData ExtractOddsValue(HtmlDocument doc)
        {
            var oddsData = new OddsData();

            // Find the element with text 'Maç Sonucu'
            var macSonucuElement = doc.DocumentNode.SelectSingleNode("//a[@class='compare-rate-bg-up' and contains(@href, 'Maç Sonucu')]");

            if (macSonucuElement != null)
            {
                var href = macSonucuElement.GetAttributeValue("href", "");
                var oddsValues = Regex.Matches(href, @"'([^']*)'").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

                if (oddsValues.Any())
                {
                    var isBasketball = oddsValues[1].Contains("Uzt");

                    if (double.TryParse(isBasketball ? oddsValues[4] : oddsValues[5], out double odds1) &&
                        double.TryParse(isBasketball ? oddsValues[5] : oddsValues[7], out double odds2))
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
