namespace FootballMatchPrediction.Core.Models;

public class MatchPredictionResult
{
    public string Prediction { get; set; }
    public List<ParsedMatch> Matches { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
    public string ActualScore { get; set; }
    public DateTime MatchDate { get; set; }
}