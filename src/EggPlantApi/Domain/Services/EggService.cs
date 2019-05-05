using System;
using EggPlantApi.Context;
using EggPlantApi.Domain.Entities;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.BulkInsert;

namespace EggPlantApi.Domain.Services
{
    public class EggService : IEggService
    {
        private readonly ILogger<EggService> _logger;
        private readonly DocumentStoreHolder _documentStoreHolder;

        public EggService(ILogger<EggService> _logger, DocumentStoreHolder documentStoreHolder)
        {
            this._logger = _logger;
            _documentStoreHolder = documentStoreHolder;
        }

        public void CreateEggs(int quantity)
        {
            try
            {
                using (BulkInsertOperation bulkInsert = _documentStoreHolder.Store.BulkInsert())
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        var egg = new Egg();
                        _logger.LogInformation($"Creating Egg: {egg.Name} : {egg.Id}");
                        bulkInsert.Store(egg);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }
    }
}
