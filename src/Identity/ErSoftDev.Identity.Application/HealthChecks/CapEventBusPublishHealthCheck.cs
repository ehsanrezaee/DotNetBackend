using DotNetCore.CAP;
using EventBus.Base.Standard;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ErSoftDev.Identity.Application.HealthChecks
{
    public class CapEventBusPublishHealthCheck : IHealthCheck
    {
        private readonly ICapPublisher _capPublisher;

        public CapEventBusPublishHealthCheck(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await _capPublisher.PublishAsync(
                    "ErSoftDev.Identity.Application.HealthChecks.RabbitMqHealthCheckIntegrationEvent",
                    new CapHealthCheckIntegrationEvent(), cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    }

    public class CapHealthCheckIntegrationEvent : IntegrationEvent
    {
    }
}
