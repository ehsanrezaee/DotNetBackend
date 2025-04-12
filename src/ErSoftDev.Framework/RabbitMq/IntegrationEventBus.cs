using DotNetCore.CAP;
using ErSoftDev.Common.Utilities;
using ErSoftDev.Framework.Configuration;
using EventBus.Base.Standard;

namespace ErSoftDev.Framework.RabbitMq
{
    public class IntegrationEventBus : IIntegrationEventBus, ITransientDependency
    {
        private readonly ICapPublisher _capPublisher;
        public IntegrationEventBus(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task PublishAsync<TEvent>(TEvent contentObject, CancellationToken cancellationToken) where TEvent : IntegrationEvent
        {
            var type = contentObject?.GetType();
            if (type?.GetCustomAttributes(typeof(FullNameAttribute), false).FirstOrDefault() is FullNameAttribute
                fullNameAttribute)
                await _capPublisher.PublishAsync(fullNameAttribute.Name, contentObject,
                    cancellationToken: cancellationToken);
        }
    }
}