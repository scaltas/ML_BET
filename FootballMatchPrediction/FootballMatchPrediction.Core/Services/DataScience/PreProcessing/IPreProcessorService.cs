using FootballMatchPrediction.Core.Models;

namespace FootballMatchPrediction.Core.Services.DataScience.PreProcessing
{
    public interface IPreProcessorService
    {
        List<PreprocessedMatch> PreProcess(string teamName, List<ParsedMatch> matchData, bool isHome);
    }
}
