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
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Run();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task Run()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var matchDataService = scope.ServiceProvider.GetRequiredService<IMatchDataService>();
            var matchPredictionService = scope.ServiceProvider.GetRequiredService<IMatchPredictionService>();
            var repository = scope.ServiceProvider.GetRequiredService<IMatchPredictionRepository>();

            await repository.DeleteAll();

            var numbers = await matchDataService.GetMatchIdsFromWebsite();
            var results = new List<MatchPredictionResult>();

            foreach (var number in numbers)
            {
                var result = matchPredictionService.PredictMatchOutcome(new MatchInputModel()
                {
                    Match = $"https://arsiv.mackolik.com/Match/Default.aspx?id={number}"
                });
                results.Add(new MatchPredictionResult()
                {
                    Id = Convert.ToInt32(number),
                    HomeTeam = result.HomeTeam,
                    AwayTeam = result.AwayTeam,
                    FirstHalfPrediction = result.FirstHalfPrediction,
                    FirstHalfActualScore = result.FirstHalfActualScore,
                    Prediction = result.Prediction,
                    ActualScore = result.ActualScore,
                    MatchDate = result.MatchDate
                });
            }

            await repository.Insert(results);
        }
    }
}