using FootballMatchPrediction.Core.Services.DataScience.Prediction;
using FootballMatchPrediction.Core.Services.DataScience.PreProcessing;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IMatchDataService, MatchDataService>();
builder.Services.AddScoped<IPreProcessorService, PreProcessorService>();
builder.Services.AddScoped<IPredictorService, PredictorService>();
builder.Services.AddScoped<IMatchPredictionService, MatchPredictionService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
