using System;
using System.Collections.Generic;
using EggPlantApi.Context;
using EggPlantApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.BulkInsert;

namespace EggPlantApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EggPlantController : ControllerBase
    {
        private readonly DocumentStoreHolder _documentStoreHolder;

        public EggPlantController(DocumentStoreHolder documentStoreHolder)
        {
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
                        bulkInsert.Store(new Egg
                        {
                            Height = egg.Height,
                            Width = egg.Width,
                            Name = egg.Name,
                            Radius = egg.Radius
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Created($"{Request.Path}/egg", eggs);
        }
    }
}