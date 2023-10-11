using System.Net;
using ErSoftDev.Framework.Redis;
using Microsoft.AspNetCore.Http;

namespace ErSoftDev.Framework.Middlewares
{
    public class RateLimitationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisService _redisService;

        public RateLimitationMiddleware(RequestDelegate next, IRedisService redisService)
        {
            _next = next;
            _redisService = redisService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var decorator = endpoint?.Metadata.GetMetadata<LimitRequests>();

            //var decorator = context.Features.Get<IEndpointFeature>().Endpoint.Metadata.GetMetadata<LimitRequests>();

            if (decorator is null)
            {
                await _next(context);
                return;
            }
            var key = GenerateClientKey(context);
            var clientStatistics = await GetClientStatisticsByKey(key);
            if (clientStatistics != null &&
                DateTime.UtcNow < clientStatistics.LastSuccessfulResponseTime.AddSeconds(decorator.TimeWindow) &&
                clientStatistics.NumberOfRequestsCompletedSuccessfully == decorator.MaxRequests)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }
            await _redisService.AddOrUpdateAsync(key, decorator, ExpiryTime.TenMinute);
            await _next(context);
        }

        private static string GenerateClientKey(HttpContext context)
            => $"{context.Request.Path}_{context.Connection.RemoteIpAddress}";

        private async Task<ClientStatistics> GetClientStatisticsByKey(string key)
        {
            return await _redisService.GetAsync<ClientStatistics>(key);
        }
    }
    public class ClientStatistics
    {
        public DateTime LastSuccessfulResponseTime { get; set; }
        public int NumberOfRequestsCompletedSuccessfully { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LimitRequests : Attribute
    {
        public int TimeWindow { get; set; }
        public int MaxRequests { get; set; }
    }


}
