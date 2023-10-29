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
using System.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewHorizon.Services.InterviewerCopilotServices.Interfaces;
using NewHorizon.Services.InterviewerCopilotServices;

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
builder.Services.AddSingleton<IClearExpiredData, OtpService>();
builder.Services.AddSingleton<IClearExpiredData, SignUpTokenService>();
builder.Services.AddSingleton<IClearExpiredData, SessionTokenManager>();
builder.Services.AddSingleton<ISignUpTokenService, SignUpTokenService>();
builder.Services.AddSingleton<IPropertyPostRepository, PropertyPostRepository>();
builder.Services.AddSingleton<IPropertyPostService, PropertyPostService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();
builder.Services.AddSingleton<ICriteriaMatching, DrinkingCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, ExperienceRangeCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, FlatTypeCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, FoodPreferenceCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, FurnishingTypeCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, PropertyTypeCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, RentAmountCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, SmokingCriteriaMatching>();
builder.Services.AddSingleton<ICriteriaMatching, TenantPreferenceCriteriaMatching>();
builder.Services.AddSingleton<IPropertySortingService, PropertySortingService>();
builder.Services.AddSingleton<ICriteriaSorting, RadiusCriteriaSorting>();
builder.Services.AddSingleton<IPropertyMatchingService, PropertyMatchingService>();
builder.Services.AddSingleton<ISearchPropertyService, SearchPropertyService>();
builder.Services.AddSingleton<IExpiredDataClearingJob, ExpiredDataClearingJob>();
builder.Services.AddSingleton<IMailingService, MailingService>();
builder.Services.AddSingleton<IInterestService, InterestService>();
builder.Services.AddSingleton<IUserInterestRepository, UserInterestRepository>();
builder.Services.AddSingleton<IGoogleSignInService, GoogleSignInService>();
builder.Services.AddSingleton<IOpenAIService, OpenAIService>();
builder.Services.AddSingleton<IGenerateAndSendPasswordService, GenerateAndSendPasswordService>();
builder.Services.AddSingleton<IShortListedPropertyRepository, ShotlistedPropertyRepository>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration.GetValue<string>("GOOGLE_AUTH_CLIENTID") ?? Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENTID");
        options.ClientSecret = builder.Configuration.GetValue<string>("GOOGLE_AUTH_CLIENT_SECRET") ?? Environment.GetEnvironmentVariable("GOOGLE_AUTH_CLIENT_SECRET");
    });
builder.Services.AddMemoryCache();
builder.Services.AddCors();
builder.Configuration.AddEnvironmentVariables().AddUserSecrets<StartupBase>();
var app = builder.Build();
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
app.UseMiddleware<ApiKeyRateLimiterMiddleware>(new MemoryCache(new MemoryCacheOptions()), TimeSpan.FromMinutes(1));
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();