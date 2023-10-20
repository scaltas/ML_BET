namespace FootballMatchPrediction.API.Models;

public class MatchPredictionResult
{
    public string Prediction { get; set; }
    public List<ParsedMatch> Matches { get; set; }
}