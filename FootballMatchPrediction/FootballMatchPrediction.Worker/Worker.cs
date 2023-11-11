using System.Reflection.Metadata.Ecma335;
using FootballMatchPrediction.Core.Models;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.Data.Repository;
using MatchPredictionResult = FootballMatchPrediction.Data.Model.MatchPredictionResult;

namespace FootballMatchPrediction.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Run();
        }

        private async Task Run()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var matchDataService = scope.ServiceProvider.GetRequiredService<IMatchDataService>();
            var matchPredictionService = scope.ServiceProvider.GetRequiredService<IMatchPredictionService>();
            var repository = scope.ServiceProvider.GetRequiredService<IMatchPredictionRepository>();

            await repository.DeleteAll();

            var numbers = await matchDataService.GetMatchIdsFromWebsite();

            var tasks = numbers.Select(async number =>
            {
                try
                {
                    var result = await matchPredictionService.PredictMatchOutcome(new MatchInputModel()
                    {
                        Match = $"https://arsiv.mackolik.com/Match/Default.aspx?id={number}"
                    });
                    
                    Console.WriteLine(number);
                    if (result.IsFailed)
                        return new MatchPredictionResult(){Id = 0};

                    return new MatchPredictionResult()
                    {
                        Id = Convert.ToInt32(number),
                        HomeTeam = result.HomeTeam,
                        AwayTeam = result.AwayTeam,
                        FirstHalfPrediction = result.FirstHalfPrediction,
                        FirstHalfActualScore = result.FirstHalfActualScore,
                        Prediction = result.Prediction,
                        ActualScore = result.ActualScore,
                        MatchDate = result.MatchDate
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new MatchPredictionResult(){Id = 0};
                }

            });

            var results = await Task.WhenAll(tasks);
            results = results.Where(r => r.Id != 0).ToArray();
            await repository.Insert(results.ToList());
        }
    }
}