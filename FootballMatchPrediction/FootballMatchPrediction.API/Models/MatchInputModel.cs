namespace FootballMatchPrediction.API.Models;

public class MatchInputModel
{
    public int TeamId { get; set; }
    public string TeamName { get; set; }
    public double Odds1 { get; set; }
    public double Odds2_5Over { get; set; }
}