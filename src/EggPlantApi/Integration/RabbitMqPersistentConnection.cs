using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EggPlantApi.Integration
{
    public class RabbitMqPersistentConnection : IRabbitMqPersistentConnection
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMqPersistentConnection> _logger;
        private IConnection _persistentConnection;
        bool _disposed;

        public RabbitMqPersistentConnection(
            IConfiguration configuration, 
            ILogger<RabbitMqPersistentConnection> logger)
        {
            _configuration = configuration;
            _logger = logger;

            ConnectionFactory cf = new ConnectionFactory
            {
                UserName = _configuration["EventBusUserName"],
                Password = _configuration["EventBusPass"],
                VirtualHost = _configuration["EventBusVirtualHost"],
                HostName = _configuration["EventBusHostName"],
                Port = int.Parse(_configuration["EventBusPort"])
            };

            var retry = new Retry
            {
                Log = methodName => { _logger.LogWarning($"Retiring action {methodName} in 2 sec"); }
            };

            retry.Execute(() =>
            {
                _persistentConnection = cf.CreateConnection();
            });
        }

        public bool IsConnected => _persistentConnection != null && _persistentConnection.IsOpen && !_disposed;

        public bool TryConnect()
        {
            throw new NotImplementedException();
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _persistentConnection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _persistentConnection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}
