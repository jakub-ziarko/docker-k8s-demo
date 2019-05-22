using System;
using RabbitMQ.Client;

namespace EggOrdersApi.Integration
{
    public interface IRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}