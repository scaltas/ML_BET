namespace FootballMatchPrediction.API.Models;

public class MatchInputModel
{
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; }
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; }
    public double Odds1 { get; set; }
    public double OddsX { get; set; }
    public double Odds2 { get; set; }
    public double Odds2_5Under { get; set; }
    public double Odds2_5Over { get; set; }
}