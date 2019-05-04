using System;
using System.Net.Http;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace EggPlantApi.Context
{
    public class DocumentStoreHolder
    {
        private readonly ILogger<DocumentStoreHolder> _logger;
        public IDocumentStore Store { get; }

        public DocumentStoreHolder(ILogger<DocumentStoreHolder> logger, IOptions<RavenSettings> ravenSettings)
        {
            _logger = logger;
            var settings = ravenSettings.Value;

            Store = new DocumentStore
            {
                Urls = settings.Url,
                Database = settings.DefaultDatabase
            }.Initialize();
        }

        public void EnsureDatabaseExists(string database = null, bool createDatabaseIfNotExists = true)
        {
            database = database ?? Store.Database;

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

            _logger.LogDebug("Trying to create DB if not present with retry policy");
            var policy = Policy
                .Handle<RavenException>()
                .Or<HttpRequestException>()
                .Or<SocketException>()
                .WaitAndRetryForever(
                    retryAttempt => TimeSpan.FromSeconds(2),
                    (ex, retryAttempt, time) =>
                    {
                        _logger.LogWarning(ex.Message);
                        _logger.LogWarning($"Trying to establish connection in {time}");
                        _logger.LogWarning($"Attempt number {retryAttempt}");
                    });

            try
            { 
                policy.Execute(() => { Store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation()); });
            }
            catch (DatabaseDoesNotExistException)
            {
                if (createDatabaseIfNotExists == false)
                    throw;

                try
                {
                    policy.Execute(() => Store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database))));
                }
                catch (ConcurrencyException)
                {
                    _logger.LogError("The database was already created before calling CreateDatabaseOperation");
                }

            }
            catch (Exception e)
            {
                _logger.LogDebug("LAST CATCH");
                _logger.LogError(e.Message, e);
            }
        }
    }
}
