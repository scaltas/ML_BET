namespace FootballMatchPrediction.Core.Models
{
    public class ParsedMatch
    {
        public required string Match { get; set; }
        public required string Score { get; set; }
        public required string FirstHalfScore { get; set; }

        public DateTime? Date { get; set; }
        public OddsData? OddsData { get; set; }
        public required string MatchUrl { get; set; }
    }
}
