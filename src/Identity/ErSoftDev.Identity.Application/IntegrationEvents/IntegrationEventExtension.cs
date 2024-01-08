using ErSoftDev.Identity.Application.HealthChecks;
using EventBus.Base.Standard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ErSoftDev.Identity.Application.IntegrationEvents
{
    public static class IntegrationEventExtension
    {
        public static IEnumerable<IIntegrationEventHandler> GetHandlers()
        {
            return new List<IIntegrationEventHandler>
            {
                new CapEventBusSubscribeHealthCheck()
            };
        }

        public static IApplicationBuilder SubscribeToEvents(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<CapHealthCheckIntegrationEvent, CapEventBusSubscribeHealthCheck>();

            return app;
        }
    }
}
