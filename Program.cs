using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using NewHorizon.CronJob;
using NewHorizon.Handlers;
using NewHorizon.Services;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services;
using SkipTrafficLib.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITrafficDataService, TrafficDataService>();
builder.Services.AddSingleton<ITrafficDataStorageService, TrafficDataStorageService>();
builder.Services.AddSingleton<IRouteRegistrationService, RouteRegistrationService>();
builder.Services.AddSingleton<IRouteRegistrationHandler, RouteRegistrationHandler>();
builder.Services.AddSingleton<IDataAccumulationJob, DataAccumulationJob>();
builder.Services.AddSingleton<IGooglePlaceService,GooglePlaceService>();
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
builder.Configuration.AddEnvironmentVariables().AddUserSecrets<StartupBase>();

var app = builder.Build();
app.Services.GetService<IDataAccumulationJob>()?.Init();

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
