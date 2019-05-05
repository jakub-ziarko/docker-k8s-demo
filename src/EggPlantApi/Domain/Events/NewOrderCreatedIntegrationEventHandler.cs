using System;
using System.Linq;
using System.Threading.Tasks;
using EggPlantApi.Context;
using EggPlantApi.Domain.Entities;
using EggPlantApi.Domain.Services;
using EggPlantApi.Integration;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.BulkInsert;

namespace EggPlantApi.Domain.Events
{
    public class NewOrderCreatedIntegrationEventHandler : IIntegrationEventHandler<NewOrderCreatedIntegrationEvent>
    {
        private readonly ILogger<NewOrderCreatedIntegrationEventHandler> _logger;
        private readonly IEggService _eggService;
        private readonly DocumentStoreHolder _documentStoreHolder;

        public NewOrderCreatedIntegrationEventHandler(
            ILogger<NewOrderCreatedIntegrationEventHandler> logger,
            IEggService eggService,
            DocumentStoreHolder documentStoreHolder)
        {
            _logger = logger;
            _eggService = eggService;
            _documentStoreHolder = documentStoreHolder;
        }

        public Task<object> Handle(NewOrderCreatedIntegrationEvent eventData)
        {
            try
            {
                var session = _documentStoreHolder.Store.OpenSession();
                int eggsCount = session.Query<Egg>().Count();

                if (eggsCount < eventData.Quantity)
                {
                    _eggService.CreateEggs(eventData.Quantity);
                }

                var eggs = session.Query<Egg>().Take(eventData.Quantity);
                var stock = new Order
                {
                    Eggs = eggs.ToList(),
                    ClientId = eventData.ClientId
                };

                using (BulkInsertOperation bulkInsert = _documentStoreHolder.Store.BulkInsert())
                {
                    _logger.LogInformation($"Creating stock order: ClientId - {stock.ClientId} for {stock.Eggs.Count} quantity");
                    bulkInsert.Store(stock);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }

            return null;
        }
    }
}
