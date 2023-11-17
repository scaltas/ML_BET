using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.Data;
using FootballMatchPrediction.Data.Repository;
using FootballMatchPrediction.PastResults.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddHostedService<Worker>();
        services.AddDbContext<MatchPredictionDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IMatchPredictionRepository, MatchPredictionRepository>();
        services.AddScoped<IMatchDataService, MatchDataService>();
    })
    .Build();

host.Run();
