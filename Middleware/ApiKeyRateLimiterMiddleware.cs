using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Models.ColleagueCastleModels;
using Newtonsoft.Json;

namespace NewHorizon.Middleware
{
    public class ApiKeyRateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _period;
        private readonly Dictionary<string, int> RateLimitedApisPerMinRate = new Dictionary<string, int>()
        {
            {"/api/PlaceSuggestion", 10},
            { "/api/OTP/generate-and-send", 2 },
            { "/api/Blob/Upload", 5 },
            { "/api/Interest/show-interest", 5 },
            { "/api/SignIn", 3 },
            { "/api/SignOut", 3 }
        };

        public ApiKeyRateLimiterMiddleware(RequestDelegate next, IMemoryCache cache, TimeSpan period)
        {
            _next = next;
            _cache = cache;
            _period = period;
        }

        public async Task Invoke(HttpContext context)
        {
            bool skipRateLimiting = true;
            if (RateLimitedApisPerMinRate.Keys.Where(api => context.Request.Path.StartsWithSegments(api)).Any())
            {
                skipRateLimiting = false;
            }

            if (skipRateLimiting)
            {
                await _next(context);
                return;
            }

            string apiKey = RateLimitedApisPerMinRate.Keys.Where(api => context.Request.Path.StartsWithSegments(api)).First();
            int limit = RateLimitedApisPerMinRate[apiKey];

            var key = context.Request.Headers["X-Api-Key"];

            if (string.IsNullOrEmpty(key))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key required in Header under X-Api-Key");
                return;
            }
            key += context.Request.Path;

            var counter = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = _period;
                return 0;
            });

            if (counter > limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync($"API rate limit exceeded ({limit} requests per minute)");
                return;
            }

            _cache.Set(key, counter + 1, _period);

            await _next(context);
        }
    }
}
