namespace FootballMatchPrediction.API.Models
{
    public class PreprocessedMatch
    {
        public double Odds { get; set; }
        public Tuple<int,int> Score { get; set; }
        public DateTime DateTime { get; set; }
    }
}
