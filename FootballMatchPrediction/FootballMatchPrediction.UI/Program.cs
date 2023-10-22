using FootballMatchPrediction.Core.Services.DataScience.Prediction;
using FootballMatchPrediction.Core.Services.DataScience.PreProcessing;
using FootballMatchPrediction.Core.Services.MatchPrediction;
using FootballMatchPrediction.Core.Services.Parse;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IMatchDataService, MatchDataService>();
builder.Services.AddScoped<IPreProcessorService, PreProcessorService>();
builder.Services.AddScoped<IPredictorService, PredictorService>();
builder.Services.AddScoped<IMatchPredictionService, MatchPredictionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MatchScore}/{action=Index}/{id?}");

app.Run();
