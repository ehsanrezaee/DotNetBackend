using Autofac;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Log;
using EventBus.Base.Standard;
using EventBus.RabbitMQ.Standard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ErSoftDev.Framework.RabbitMq
{
    public static class Registration
    {
        public static IServiceCollection? AddRabbitMqRegistration(this IServiceCollection? services, AppSetting appSetting)
        {
            if (appSetting.EventBusRabbitMq is null)
                return null;

            services.AddSingleton<IEventBus, EventBusRabbitMqService>(sp =>
            {
                var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMqService>>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var appSetting = sp.GetRequiredService<IOptions<AppSetting>>();
                var lifeTimeScope = sp.GetRequiredService<ILifetimeScope>();

                var brokerName = appSetting.Value.EventBusRabbitMq.BrokerName;
                var queueName = appSetting.Value.EventBusRabbitMq.QueueName;
                var retryCount = appSetting.Value.EventBusRabbitMq.TryCount;
                var preFetchCount = appSetting.Value.EventBusRabbitMq.PreFetchCount;

                return new EventBusRabbitMqService(appSetting,
                    rabbitMqPersistentConnection,
                    eventBusSubscriptionsManager,
                    lifeTimeScope,
                    brokerName,
                    logger,
                    serviceScopeFactory,
                    queueName,
                    retryCount,
                    preFetchCount
                    );
            });
            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
            return services;
        }
    }
}
