using System;
using System.Collections.Generic;
using EggPlantApi.Context;
using EggPlantApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.BulkInsert;

namespace EggPlantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EggPlantController : ControllerBase
    {
        private readonly ILogger<EggPlantController> _logger;
        private readonly DocumentStoreHolder _documentStoreHolder;

        public EggPlantController(ILogger<EggPlantController> logger, DocumentStoreHolder documentStoreHolder)
        {
            _logger = logger;
            _documentStoreHolder = documentStoreHolder;
        }

        [HttpGet("{id}")]
        public IActionResult GetEgg(int id)
        {
            return Ok(new Egg
            {
                Id = Guid.NewGuid().ToString(),
                Name = "EggBlack",
                Height = 2,
                Radius = 2,
                Width = 2
            });
        }

        [HttpPost]
        public IActionResult CreateEgg(IEnumerable<Egg> eggs)
        {
            try
            {
                using (BulkInsertOperation bulkInsert = _documentStoreHolder.Store.BulkInsert())
                {
                    foreach (var egg in eggs)
                    {
                        _logger.LogInformation($"Inserting: {egg.Name} : {egg.Id}");
                        bulkInsert.Store(egg);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return StatusCode(500);
            }

            return Created($"{Request.Path}/egg", eggs);
        }
    }
}