using DotNetCore.CAP;
using EventBus.Base.Standard;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.Identity.Application.HealthChecks
{
    public class CapEventBusSubscribeHealthCheck : IHealthCheck, IIntegrationEventHandler<CapHealthCheckIntegrationEvent>, ICapSubscribe
    {
        [CapSubscribe("ErSoftDev.Identity.Application.HealthChecks.RabbitMqHealthCheckIntegrationEvent")]
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return HealthCheckResult.Healthy();
        }

        public async Task Handle(CapHealthCheckIntegrationEvent @event)
        {

        }
    }
}
