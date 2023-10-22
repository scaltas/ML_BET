using FootballMatchPrediction.Core.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.Core.Services.Parse
{
    public interface IParser
    {
        List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName);
    }
}
