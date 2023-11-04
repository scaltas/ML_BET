using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.Parse;

public interface IMatchDataService
{
    List<ParsedMatch> ScrapeMatchData(string url, string teamName);
    string[] GetTeamUrls(string matchUrl);
    Task<string[]> GetMatchIdsFromWebsite();
}