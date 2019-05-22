using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EggPlantApi.Integration
{
    public class EventBusRabbitMq : IEventBus
    {
        const string BrokerName = "event_bus";

        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, SubscribedEvents> _subscribedEvents = new Dictionary<string, SubscribedEvents>();
        private readonly IModel _consumer;
        private EventingBasicConsumer consumer;

        public EventBusRabbitMq(
            IRabbitMqPersistentConnection persistentConnection,
            IServiceProvider serviceProvider)
        {
            _persistentConnection = persistentConnection;
            _serviceProvider = serviceProvider;
            _consumer = CreateConsumerChannel();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {
                var eventName = @event.GetType().Name;

                channel.ExchangeDeclare(exchange: BrokerName,
                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                channel.BasicPublish(exchange: BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            }
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventType = typeof(T);
            var eventName = eventType.Name;
            var knownEvent = _subscribedEvents.ContainsKey(eventName);
            var alreadySubscribed =  knownEvent && _subscribedEvents[eventName].Handlers.Contains(typeof(TH));
            if (!alreadySubscribed)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                if (!knownEvent)
                {
                    _consumer.QueueDeclare(queue: eventName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    _consumer.BasicConsume(queue: eventName,
                        consumer: consumer);
                    _subscribedEvents.Add(eventName, new SubscribedEvents());
                    _subscribedEvents[eventName].EventType = eventType;
                }

                _consumer.QueueBind(queue: eventName,
                    exchange: BrokerName,
                    routingKey: eventName);
                _subscribedEvents[eventName].Handlers.Add(typeof(TH));
            }
        }

        public void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {

        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: BrokerName,
                type: "direct");
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subscribedEvents.TryGetValue(eventName, out var subscribedEvents))
            {
                foreach (var handlerType in subscribedEvents.Handlers)
                {
                    try
                    {
                        var @event = _serviceProvider.GetRequiredService(subscribedEvents.EventType);
                        var integrationEvent = JsonConvert.DeserializeObject(message, @event.GetType());
                        var handler = _serviceProvider.GetRequiredService(handlerType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(subscribedEvents.EventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                    }
                    catch (Exception e)
                    {
                        // to do log
                    }
                }
            }
        }
    }
}