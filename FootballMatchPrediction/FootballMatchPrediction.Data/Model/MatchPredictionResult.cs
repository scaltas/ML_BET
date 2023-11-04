namespace FootballMatchPrediction.Data.Model
{
    public class MatchPredictionResult
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string FirstHalfPrediction { get; set; }
        public string FirstHalfActualScore { get; set; }
        public string Prediction { get; set; }
        public string ActualScore { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
