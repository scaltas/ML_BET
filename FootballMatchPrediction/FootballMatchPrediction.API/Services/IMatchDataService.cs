using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services;

public interface IMatchDataService
{
    List<ParsedMatch> ScrapeMatchData(int teamId, string teamName, int numberOfSeasonsToRetrieve);
}