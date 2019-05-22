using System;
using EggOrdersApi.Context;
using EggOrdersApi.Events;
using EggOrdersApi.Integration;
using EggOrdersApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Commands;
using Raven.Client.Documents.Session;

namespace EggOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly DocumentStoreHolder _documentStoreHolder;
        private readonly IEventBus _eventBus;

        public OrderController(
            ILogger<OrderController> logger, 
            DocumentStoreHolder documentStoreHolder,
            IEventBus eventBus)
        {
            _logger = logger;
            _documentStoreHolder = documentStoreHolder;
            _eventBus = eventBus;
        }

        public IActionResult Post([FromBody] Order order)
        {
            try
            {
                var docInfo = new DocumentInfo
                {
                    Collection = "Orders"
                };
                var session = _documentStoreHolder.Store.OpenSession();
                var document = session.Advanced.EntityToBlittable.ConvertEntityToBlittable(order, docInfo);
                var command = new PutDocumentCommand(order.OrderId, null, document);

                session.Advanced.RequestExecutor.Execute(command, session.Advanced.Context);

                _logger.LogInformation($"Order {order.OrderId} for {order.ClientName} {order.ClientSurname} Created");

                _eventBus.Publish(new NewOrderCreatedIntegrationEvent
                {
                    OrderId = order.OrderId,
                    ClientId = $"{order.ClientName}//{order.ClientSurname}",
                    Quantity = order.Quantity
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e.Message);
            }

            return Created("", order);
        }
    }
}