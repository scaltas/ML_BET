namespace FootballMatchPrediction.UI.Models
{
    public class MatchScore
    {
        public int MatchId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string PredictedScore { get; set; }
        public string ActualScore { get; set; }
        public DateTime MatchDate { get; set; }
        public string FirstHalfScore { get; set; }
    }
}
