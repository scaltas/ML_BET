using FootballMatchPrediction.API.Models;
using HtmlAgilityPack;

namespace FootballMatchPrediction.API.Services.Parse
{
    public interface IParser
    {
        List<ParsedMatch> ParseDoc(HtmlDocument doc, string teamName);
    }
}
