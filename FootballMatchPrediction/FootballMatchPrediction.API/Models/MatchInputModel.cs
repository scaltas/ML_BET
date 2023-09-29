namespace FootballMatchPrediction.API.Models;

public class MatchInputModel
{
    public required string Match { get; set; }
    public string? HomeTeam { get; set; }
    public string? AwayTeam { get; set; }
}