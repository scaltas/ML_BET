namespace FootballMatchPrediction.Core.Models;

public class MatchInputModel
{
    public required string Match { get; set; }
    public string Algorithm { get; set; } = "LinearRegression";
    public int SampleCount { get; set; } = 10;
}
