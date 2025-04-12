using System.Net.Sockets;
using System.Text;
using Autofac;
using ErSoftDev.Framework.BaseApp;
using ErSoftDev.Framework.Log;
using EventBus.Base.Standard;
using EventBus.RabbitMQ.Standard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace ErSoftDev.Framework.RabbitMq
{
    public class EventBusRabbitMqService : IEventBus, IDisposable
    {

        private readonly IOptions<AppSetting> _appSetting;
        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly ILifetimeScope _autofac;
        private readonly string _brokerName;
        private readonly int _retryCount;
        private readonly int _preFetchCount;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventBusRabbitMqService> _logger;
        private readonly ILifetimeScope _lifetimeScope;

        private string _queueName;
        private IModel _consumerChannel;
        private IModel _channel;

        public EventBusRabbitMqService(IOptions<AppSetting> appSetting,
            IRabbitMqPersistentConnection persistentConnection,
            IEventBusSubscriptionManager subsManager,
            ILifetimeScope lifetimeScope,
            string brokerName,
            ILogger<EventBusRabbitMqService> logger,
            IServiceScopeFactory serviceScopeFactory,
            string queueName = null,
            int retryCount = 5,
            int preFetchCount = 1
        )
        {
            _appSetting = appSetting;
            _persistentConnection = persistentConnection;
            _queueName = queueName;
            _brokerName = brokerName;
            _retryCount = retryCount;
            _preFetchCount = preFetchCount;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _lifetimeScope = lifetimeScope;
            _subsManager = subsManager;
            _consumerChannel = CreateConsumerChannel();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected)
                //_persistentConnection.TryConnect();
                return;


            using var channel = _persistentConnection.CreateModel();
            channel.QueueUnbind(_queueName, _brokerName, eventName);

            if (!_subsManager.IsEmpty)
                return;

            _queueName = string.Empty;
            _consumerChannel.Close();
        }
        public async void Publish(IntegrationEvent @event)
        {
            //Get try count
            var retryCount = _appSetting.Value.EventBusRabbitMq.TryCount;

            // this is for check persistent connection is ok or not and if not ok retry no limited to connect
            //if (!_persistentConnection.IsConnected)
            //    _persistentConnection.TryConnect();


            // use poly to config repeatable oprations
            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { });

            //Crate Connection 
            using var channel = _persistentConnection.CreateModel();

            channel.QueueDeclare(
                ((BasicEvent)@event).Queue.ToString(),
                true,
                false,
                false,
                null
            );

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                var jsonRequest = JsonConvert.SerializeObject(@event);
                properties.Headers = new Dictionary<string, object>()
                {
                    { "EventName", @event.GetType().Name },
                };

                properties.ContentType = "Application/Json";

                channel.BasicPublish("", ((BasicEvent)@event).Queue.ToString(), properties,
                    Encoding.UTF8.GetBytes(jsonRequest));

            });
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoInternalSubscription(eventName);

            _subsManager.AddDynamicSubscription<TH>(eventName);

            Consume();
        }
        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            DoInternalSubscription(eventName);

            _subsManager.AddSubscription<T, TH>();

            Consume();
        }
        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (containsKey)
                return;

            if (!_persistentConnection.IsConnected)
                //_persistentConnection.TryConnect();
                return;


            using var channel = _persistentConnection.CreateModel();
            channel.QueueBind(_queueName, _brokerName, eventName);

        }
        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }
        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }
        public void Dispose()
        {
            _consumerChannel?.Dispose();

            _subsManager.Clear();
        }
        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
                return null;

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(_brokerName, "direct", true);
            channel.QueueDeclare(_queueName, true, false, false, null);
            channel.BasicQos(0, (ushort)_preFetchCount, false);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();

                Consume();
            };

            return channel;
        }
        private void Consume()
        {
            if (_consumerChannel is null)
                return;

            var consumer = new EventingBasicConsumer(_consumerChannel);
            consumer.Received += ProcessEvent;
            _consumerChannel.BasicRecover(true);
            _consumerChannel.BasicConsume(_queueName, true, consumer);
        }
        private async void ProcessEvent(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                if (e.Redelivered)
                    return;

                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                var eventName = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers
                    .FirstOrDefault(header => header.Key == "EventName").Value);

                if (!_subsManager.HasSubscriptionsForEvent(eventName))
                    return;

                await using var scope = _lifetimeScope.BeginLifetimeScope("Scope");
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler
                                handler))
                        {
                            continue;
                        }

                        dynamic eventData = JObject.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null)
                        {
                            continue;
                        }

                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogInformation("RabbitMqException message : {0}", exception.Message);
            }
        }

        public int GetQueueMessageCount()
        {
            if (!_persistentConnection.IsConnected)
                return 0;
            //_persistentConnection.TryConnect();

            using var channel = _persistentConnection.CreateModel();
            return (int)channel.MessageCount(_queueName);
        }
    }

    /// <summary>
    /// request publish for which queue , by select any queue , subscriber check selected queue
    /// </summary>
    public enum Queue
    {
        MessageQueue,
        Identity,
        Wallex
    }

    public class BasicEvent : IntegrationEvent
    {
        public BasicEvent(Queue queue)
        {

            Queue = queue;
        }
        public Queue Queue { get; }
    }
}
