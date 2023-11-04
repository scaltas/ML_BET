using FootballMatchPrediction.Core.Services.DataScience.Prediction;
using FootballMatchPrediction.Core.Services.DataScience.PreProcessing;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;
using FootballMatchPrediction.Data;
using FootballMatchPrediction.Data.Repository;
using FootballMatchPrediction.Worker;
using Microsoft.EntityFrameworkCore;

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
        services.AddScoped<IMatchPredictionService, MatchPredictionService>();
        services.AddScoped<IPreProcessorService, PreProcessorService>();
        services.AddScoped<IPredictorService, PredictorService>();
    })
    .Build();


host.Run();
