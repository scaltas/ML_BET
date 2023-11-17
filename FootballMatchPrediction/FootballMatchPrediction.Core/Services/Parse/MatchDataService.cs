using FootballMatchPrediction.Core.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.Core.Services.Parse
{
    public class MatchDataService : IMatchDataService
    {
        public async Task<List<ParsedMatch>> ScrapeMatchData(string teamUrl, string teamName)
        {
            var matchData = new List<ParsedMatch>();

            var web = new HtmlWeb();
            
            if (!teamUrl.Contains("https"))
                teamUrl = $"https:{teamUrl}";
            
            var doc = await web.LoadFromWebAsync(teamUrl);

            matchData.AddRange(ParseDoc(doc, teamName));

            return matchData;
        }

        public async Task<string[]> GetTeamUrls(string matchUrl)
        {
            var result = new List<string>();

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(matchUrl);

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

        public async Task<string[]> GetMatchIdsFromWebsite()
        {
            using HttpClient client = new HttpClient();
            string htmlContent = await client.GetStringAsync("https://arsiv.mackolik.com/Program/Program.aspx");

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@href, 'popMatch')]");
            if (linkNodes == null)
            {
                return Array.Empty<string>();
            }

            return linkNodes.Select(node => ExtractNumber(node.GetAttributeValue("href", ""))).ToArray();
        }
        private string ExtractNumber(string input)
        {
            int startIndex = input.IndexOf("popMatch(");
            int endIndex = input.IndexOf(",\"ByLeague\"", startIndex);

            if (startIndex != -1 && endIndex != -1)
            {
                startIndex += "popMatch(".Length;
                return input.Substring(startIndex, endIndex - startIndex);
            }

            return string.Empty;
        }

        private List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName)
        {
            var parser = ParserFactory.GetParser(doc);
            return parser.ParseDoc(doc, teamName);
        }

        public async Task<string?> GetScore(string matchUrl)
        {
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(matchUrl);
            var scoreNode = doc.DocumentNode.SelectSingleNode("//div[@class='match-score']");

            var scoreText = scoreNode?.InnerText.Trim();
            return scoreText;

        }
    }
}