using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NewHorizon.CronJob;
using NewHorizon.Handlers;
using NewHorizon.Repositories;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services;
using NewHorizon.Services.ColleagueCastleServices;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services;
using SkipTrafficLib.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2" });
});
builder.Services.AddSingleton<ITrafficDataService, TrafficDataService>();
builder.Services.AddSingleton<ITrafficDataStorageService, TrafficDataStorageService>();
builder.Services.AddSingleton<IRouteRegistrationService, RouteRegistrationService>();
builder.Services.AddSingleton<IRouteRegistrationHandler, RouteRegistrationHandler>();
builder.Services.AddSingleton<IDataAccumulationJob, DataAccumulationJob>();
builder.Services.AddSingleton<IGooglePlaceService, GooglePlaceService>();
builder.Services.AddSingleton<ISecretService, SecretService>();
builder.Services.AddSingleton<ICosmosDbService, CosmosDbService>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ISessionTokenManager, SessionTokenManager>();
builder.Services.AddSingleton<IOtpService, OtpService>();
builder.Services.AddSingleton<ISignUpTokenService, SignUpTokenService>();
builder.Configuration.AddEnvironmentVariables().AddUserSecrets<StartupBase>();

var app = builder.Build();
app.Services.GetService<IDataAccumulationJob>()?.Init();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
