using ErSoftDev.ApiGateway.Infrastructure.ServiceProviderConfiguration.Identity;
using ErSoftDev.DomainSeedWork;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.ApiGateway.Application.HealthChecks
{
    public class IdentityGrpcServiceHealthCheck : IHealthCheck
    {
        private readonly IAccountService _accountService;

        public IdentityGrpcServiceHealthCheck(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _accountService.IsSecurityStampTokenValid("any security token");
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
