﻿namespace FootballMatchPrediction.API.Models
{
    public class ParsedMatch
    {
        public required string Match { get; set; }
        public required string Score { get; set; }


        public DateTime? Date { get; set; }
        public required OddsData OddsData { get; set; }
    }
}
