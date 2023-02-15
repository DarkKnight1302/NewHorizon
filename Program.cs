using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NewHorizon.CronJob;
using NewHorizon.Handlers;
using NewHorizon.Repositories;
using NewHorizon.Services.ColleagueCastleServices;
using NewHorizon.Services.ColleagueCastleServices.Interfaces;
using NewHorizon.Repositories.Interfaces;
using NewHorizon.Services;
using NewHorizon.Services.Interfaces;
using SkipTrafficLib.Services;
using SkipTrafficLib.Services.Interfaces;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Middleware;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Colleague Castle APIs", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2" });
    c.OperationFilter<AddApiKeyHeaderParameter>();
});
builder.Services.AddSwaggerGenNewtonsoftSupport();
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
builder.Services.AddSingleton<IPropertyPostRepository, PropertyPostRepository>();
builder.Services.AddSingleton<IPropertyPostService, PropertyPostService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddMemoryCache();
builder.Services.AddCors();
builder.Configuration.AddEnvironmentVariables().AddUserSecrets<StartupBase>();
var app = builder.Build();
app.Services.GetService<IDataAccumulationJob>()?.Init();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
});

app.UseCors(builder =>
{
    builder.WithOrigins("https://www.colleaguecastle.in", "https://localhost:7280")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
});
app.UseMiddleware<ApiKeyRateLimiterMiddleware>(new MemoryCache(new MemoryCacheOptions()),  5,  TimeSpan.FromMinutes(5));
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();