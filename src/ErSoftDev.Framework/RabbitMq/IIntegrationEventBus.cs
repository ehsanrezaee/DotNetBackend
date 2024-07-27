using EventBus.Base.Standard;

namespace ErSoftDev.Framework.RabbitMq
{
    public interface IIntegrationEventBus
    {
        Task PublishAsync<TEvent>(TEvent contentObject, CancellationToken cancellationToken) where TEvent : IntegrationEvent;
    }
}
