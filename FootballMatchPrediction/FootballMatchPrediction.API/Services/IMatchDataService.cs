using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services;

public interface IMatchDataService
{
    List<ParsedMatch> ScrapeMatchData(string url, string teamName);
    string[] GetTeamUrls(string matchUrl);
}