using System;
using RabbitMQ.Client;

namespace EggPlantApi.Integration
{
    public interface IRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}