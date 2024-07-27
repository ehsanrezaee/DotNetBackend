using DotNetCore.CAP;

namespace ErSoftDev.Framework.RabbitMq
{
    public class PublishIntegrationEvent<T> : ICapPublisher
    {
        public Task PublishAsync<T>(string name, T? contentObj, string? callbackName = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task PublishAsync<T>(string name, T? contentObj, IDictionary<string, string?> headers,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(string name, T? contentObj, string? callbackName = null)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(string name, T? contentObj, IDictionary<string, string?> headers)
        {
            throw new NotImplementedException();
        }

        public Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task PublishDelayAsync<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, IDictionary<string, string?> headers)
        {
            throw new NotImplementedException();
        }

        public void PublishDelay<T>(TimeSpan delayTime, string name, T? contentObj, string? callbackName = null)
        {
            throw new NotImplementedException();
        }

        public IServiceProvider ServiceProvider { get; }
        public AsyncLocal<ICapTransaction> Transaction { get; }
    }
}
