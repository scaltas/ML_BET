using FootballMatchPrediction.API.Models;

namespace FootballMatchPrediction.API.Services.DataScience.PreProcessing
{
    public interface IPreProcessorService
    {
        List<PreprocessedMatch> PreProcess(string teamName, List<ParsedMatch> matchData, bool isHome);
    }
}
