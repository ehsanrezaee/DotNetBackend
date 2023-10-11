using ErSoftDev.Framework.BaseApp;
using EventBus.RabbitMQ.Standard;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace ErSoftDev.Framework.RabbitMq
{
    public static class Connection
    {
        public static IServiceCollection? AddRabbitMqConnection(this IServiceCollection services, AppSetting appSetting)
        {
            if (appSetting.EventBusRabbitMq is null)
                return null;

            services.AddSingleton<IRabbitMqPersistentConnection>(_ =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = appSetting.EventBusRabbitMq.HostName,
                    DispatchConsumersAsync = appSetting.EventBusRabbitMq.DispatchConsumerAsync,
                    VirtualHost = appSetting.EventBusRabbitMq.VirtualHost,
                    UserName = appSetting.EventBusRabbitMq.Username,
                    Password = appSetting.EventBusRabbitMq.Password,
                    Port = appSetting.EventBusRabbitMq.VirtualPort
                };

                var retryCount = appSetting.EventBusRabbitMq.TryCount;

                return new DefaultRabbitMqPersistentConnection(factory, retryCount);
            });

            return services;
        }
    }
}
