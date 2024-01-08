using ErSoftDev.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.Identity.Application.HealthChecks
{
    public class DataBaseHealthCheck : IHealthCheck
    {
        private readonly IdentityDbContext _identityDbContext;

        public DataBaseHealthCheck(IdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                _identityDbContext.Database.SqlQuery<int>($"SELECT 1");
                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
