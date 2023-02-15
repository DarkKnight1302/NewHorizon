﻿using Microsoft.Extensions.Caching.Memory;
using NewHorizon.Models.ColleagueCastleModels;
using Newtonsoft.Json;

namespace NewHorizon.Middleware
{
    public class ApiKeyRateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _limit;
        private readonly TimeSpan _period;

        public ApiKeyRateLimiterMiddleware(RequestDelegate next, IMemoryCache cache, int limit, TimeSpan period)
        {
            _next = next;
            _cache = cache;
            _limit = limit;
            _period = period;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/OTP/generate-and-send"))
            {
                await _next(context);
                return;
            }

            var key = context.Request.Headers["X-Api-Key"];

            if (string.IsNullOrEmpty(key))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API key required in Header under X-Api-Key");
                return;
            }

            var counter = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = _period;
                return 0;
            });

            if (counter >= _limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync($"API rate limit exceeded ({_limit} requests per {_period.TotalMinutes} seconds)");
                return;
            }

            _cache.Set(key, counter + 1, _period);

            await _next(context);
        }
    }
}