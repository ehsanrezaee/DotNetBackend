using System.Text;
using Autofac;
using ErSoftDev.Framework.BaseApp;
using EventBus.Base.Standard;
using EventBus.RabbitMQ.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ErSoftDev.Framework.Middlewares
{
    public class EventBusConsumerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRabbitMqPersistentConnection _rabbitMqPersistentConnection;
        private readonly IOptions<AppSetting> _appSetting;
        private readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;
        private readonly ILifetimeScope _lifetimeScope;

        public EventBusConsumerMiddleware(RequestDelegate next,
            IRabbitMqPersistentConnection rabbitMqPersistentConnection, IOptions<AppSetting> appSetting,
            IEventBusSubscriptionManager eventBusSubscriptionManager,
            ILifetimeScope lifetimeScope)
        {
            _next = next;
            _rabbitMqPersistentConnection = rabbitMqPersistentConnection;
            _appSetting = appSetting;
            _eventBusSubscriptionManager = eventBusSubscriptionManager;
            _lifetimeScope = lifetimeScope;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            if (!_rabbitMqPersistentConnection.IsConnected)
            {
                _rabbitMqPersistentConnection.TryConnect();
            }

            var queueName = _appSetting.Value.EventBusRabbitMq.QueueName;// _appSetting.Value.Configurations
                                                                         //.FirstOrDefault(configuration => configuration.Name == "EventBusQueueName").Value;

            using var channel = _rabbitMqPersistentConnection.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ProcessEvent;
            channel.BasicConsume(queueName, true, consumer);



            await _next(httpContext);
        }

        private async void ProcessEvent(object sender, BasicDeliverEventArgs e)
        {

            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var eventName = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers
                .FirstOrDefault(header => header.Key == "EventName").Value);

            if (_eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope("scopname"))
                {
                    var subscriptions = _eventBusSubscriptionManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler handler))
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

                            var eventType = _eventBusSubscriptionManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                        }
                    }
                }
            }
        }
    }
}
