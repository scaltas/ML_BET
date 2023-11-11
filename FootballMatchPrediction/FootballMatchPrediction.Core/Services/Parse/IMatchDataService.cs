using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.Parse;

public interface IMatchDataService
{
    Task<List<ParsedMatch>> ScrapeMatchData(string url, string teamName);
    Task<string[]> GetTeamUrls(string matchUrl);
    Task<string[]> GetMatchIdsFromWebsite();
}