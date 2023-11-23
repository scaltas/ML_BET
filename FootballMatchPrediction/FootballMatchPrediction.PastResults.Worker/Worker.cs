using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace FootballMatchPrediction.PastResults.Worker
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
            await Task.Delay(1000, stoppingToken);

            await Run();
        }

        private async Task Run()
        {
            using var scope = _serviceProvider.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IMatchPredictionRepository>();
            var matchDataService = scope.ServiceProvider.GetRequiredService<IMatchDataService>();

            var matchesQueryable = repository.GetAllPredictions();
            var matches = await matchesQueryable.ToListAsync();

            foreach (var match in matches)
            {
                if (string.IsNullOrEmpty(match.ActualScore) || match.ActualScore == "v")
                {
                    var url = $"https://arsiv.mackolik.com/Mac/{match.Id}/";
                    var score = await matchDataService.GetScore(url);

                    match.ActualScore = score;
                    Console.WriteLine($"{match.HomeTeam} {score} {match.AwayTeam}");
                    await repository.Update(match);
                }
            }

            Console.WriteLine("Completed.");
        }
    }
}