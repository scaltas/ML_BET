using HtmlAgilityPack;

namespace FootballMatchPrediction.API.Services.Parse
{
    public static class ParserFactory
    {
        public static IParser GetParser(HtmlDocument doc)
        {
            var isBasketball = doc.Text.Contains("Basketball");

            if (isBasketball)
                return new BasketballParser();
            return new FootballParser();
        }
    }
}
