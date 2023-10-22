using FootballMatchPrediction.Core.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.Core.Services.Parse
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
            var parser = ParserFactory.GetParser(doc);
            return parser.ParseDoc(doc, teamName);
        }
    }
}