using ErSoftDev.Framework.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.Identity.Application.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IRedisService _redisService;

        public RedisHealthCheck(IRedisService redisService)
        {
            _redisService = redisService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _redisService.AddOrUpdateAsync("TestKey", 1, TimeSpan.FromSeconds(10));
            return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
